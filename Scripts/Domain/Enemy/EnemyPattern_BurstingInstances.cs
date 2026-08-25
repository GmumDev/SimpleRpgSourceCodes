using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class EnemyPattern_BurstingInstances: EnemyPattern
{
	[SerializeField]
	int instanceCount;
	[SerializeField]
	float speed;
	[SerializeField]
	Mesh mesh;
	[SerializeField]
	Material mat;
	[SerializeField]
	Vector3 burstOffset;

	NativeArray<float3> directions;
	NativeArray<float3> positions;
	NativeArray<Matrix4x4> matrices;

	RenderParams renderParams;
	bool isPlaying;
	const int batchCnt = 64;
	public override void DoPattern()
	{
		SafeDispose();

		directions = new NativeArray<float3>(instanceCount, Allocator.Persistent);
		positions = new NativeArray<float3>(instanceCount, Allocator.Persistent);
		matrices = new NativeArray<Matrix4x4>(instanceCount, Allocator.Persistent);

		var initPosJob = new InitPosJob
		{
			pos = transform.position + burstOffset,
			positions = positions
		};
		initPosJob.Schedule(instanceCount, batchCnt).Complete();

		renderParams = new RenderParams(mat);

		var job = new RandomVectorGenerateJob
		{
			directions = directions,
			seed = (uint)Time.time
		};
		job.Schedule().Complete();
		isPlaying = true;
	}
	public override void CancelPattern()
	{
		isPlaying = false;
		SafeDispose();
	}
	private void LateUpdate()
	{
		if (!isPlaying) return;

		var moveJob = new MoveJob
		{
			directions = directions,
			positions = positions,
			matrices = matrices,
			deltaTime = Time.deltaTime,
			speed = speed,
			gravity = Physics.gravity.y
		};
		moveJob.Schedule(arrayLength: instanceCount, innerloopBatchCount: batchCnt).Complete();

		Graphics.RenderMeshInstanced(renderParams, mesh, 0, matrices);
	}

	[BurstCompile]
	private struct RandomVectorGenerateJob : IJob
	{
		[WriteOnly]
		public NativeArray<float3> directions;
		[ReadOnly] public uint seed;
		public void Execute()
		{
			var rnd = new Unity.Mathematics.Random(seed);
			for (int i = 0; i < directions.Length; i++)
			{
				directions[i] = rnd.NextFloat3Direction();
			}
		}
	}
	[BurstCompile]
	private struct InitPosJob : IJobParallelFor
	{
		[WriteOnly] public NativeArray<float3> positions;
		[ReadOnly] public float3 pos;
		public void Execute(int index)
		{
			positions[index] = pos;
		}
	}
	[BurstCompile]
	private struct MoveJob : IJobParallelFor
	{
		public NativeArray<float3> directions;
		public NativeArray<float3> positions;
		[WriteOnly] public NativeArray<Matrix4x4> matrices;

		[ReadOnly] public float deltaTime;
		[ReadOnly] public float speed;
		[ReadOnly] public float gravity;
		public void Execute(int i)
		{
			float3 velocity = directions[i] * speed;
			velocity += gravity * deltaTime;

			positions[i] += velocity * deltaTime;
			directions[i] = math.normalize(velocity);
			quaternion rotation = quaternion.LookRotationSafe(velocity, math.up());

			matrices[i] = Matrix4x4.TRS(positions[i], rotation, Vector3.one);
		}
	}
	private void OnDestroy()
	{
		SafeDispose();
	}
	void SafeDispose()
	{
		if (positions.IsCreated)
		{
			positions.Dispose();
			positions = default;
		}
		if (directions.IsCreated)
		{
			directions.Dispose();
			directions = default;
		}
		if (matrices.IsCreated)
		{
			matrices.Dispose();
			matrices = default;
		}
	}
}