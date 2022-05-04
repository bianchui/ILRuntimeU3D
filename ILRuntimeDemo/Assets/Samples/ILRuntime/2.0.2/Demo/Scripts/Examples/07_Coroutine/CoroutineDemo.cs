using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Runtime.Enviorment;

public class CoroutineDemo : ILRTBase
{
    static CoroutineDemo instance;
    public static CoroutineDemo Instance
    {
        get { return instance; }
    }

    void Start()
    {
        Debug.Log("CoroutineDemo.Start()");
        instance = this;
        base.Start();
    }


    protected override void InitializeILRuntime()
    {
        base.InitializeILRuntime();
        //这里做一些ILRuntime的注册
        //使用Couroutine时，C#编译器会自动生成一个实现了IEnumerator，IEnumerator<object>，IDisposable接口的类，因为这是跨域继承，所以需要写CrossBindAdapter（详细请看04_Inheritance教程），Demo已经直接写好，直接注册即可
        appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        appdomain.DebugService.StartDebugService(56000);
    }

    protected override unsafe void OnHotFixLoaded()
    {
        appdomain.Invoke("HotFix_Project.TestCoroutine", "RunTest", null, null);
    }

    public void DoCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
