using UnityEngine;
using Unity.Jobs;           //Jobs System
using Unity.Collections;
using Unity.Burst;          //Burst Compiler

public class Example : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DoExample();
    }

    private void DoExample()
    {
        NativeArray<float> resultArray = new NativeArray<float>(1, Allocator.TempJob);

        //Instantiate a job
        SimpleJob myJob = new SimpleJob
        {
            //Initialize data to work with
            a = 5f,
            result = resultArray
        };

        AnotherJob secondJob = new AnotherJob();
        secondJob.result = resultArray;



        //Initialize data to work with
        //myJob.a = 5f;
        //myJob.result = new NativeArray<float>(1, Allocator.TempJob);



        //Schedule the job
        JobHandle handle = myJob.Schedule();
        JobHandle secondHandle = secondJob.Schedule(handle);



        //Other tasks within the main thread
        //handle.Complete();
        secondHandle.Complete();

        float resultingValue = resultArray[0];
        Debug.Log("Result: " + resultingValue);

        resultArray.Dispose();
    }

    [BurstCompile(CompileSynchronously = true)]
    private struct SimpleJob : IJob
    {
        public float a;
        public NativeArray<float> result;

        public void Execute()
        {
            result[0] = a;
            a = 23f;          //Value shouldn't change the result of the process as everything is done on copies of the data
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    private struct AnotherJob : IJob
    {
        public NativeArray<float> result;

        public void Execute()
        {
            result[0] = result[0] + 1;
        }
    }
}
