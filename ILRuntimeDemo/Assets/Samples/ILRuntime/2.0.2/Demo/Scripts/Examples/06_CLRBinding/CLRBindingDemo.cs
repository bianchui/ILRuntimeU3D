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
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

public class CLRBindingTestClass
{
    public static float DoSomeTest(int a, float b)
    {
        return a + b;
    }
}

public class CLRBindingDemo : ILRTBase
{
    protected override void InitializeILRuntime()
    {
        base.InitializeILRuntime();
        //这里做一些ILRuntime的注册，如委托适配器，值类型绑定等等


        //初始化CLR绑定请放在初始化的最后一步！！
        //初始化CLR绑定请放在初始化的最后一步！！
        //初始化CLR绑定请放在初始化的最后一步！！

        //请在生成了绑定代码后解除下面这行的注释
        //请在生成了绑定代码后解除下面这行的注释
        //请在生成了绑定代码后解除下面这行的注释
        //ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
    }

    protected override unsafe void OnHotFixLoaded()
    {
        ilruntimeReady = true;
    }

    bool ilruntimeReady = false;
    bool executed = false;
    void Update()
    {
        if (ilruntimeReady && !executed && Time.realtimeSinceStartup > 3)
        {
            executed = true;
            //这里为了方便看Profiler，代码挪到Update中了
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            Debug.LogWarning("运行这个Demo前请先点击菜单ILRuntime->Generate来生成所需的绑定代码，并按照提示解除下面相关代码的注释");
            Debug.Log("默认情况下，从热更DLL里调用Unity主工程的方法，是通过反射的方式调用的，这个过程中会产生GC Alloc，并且执行效率会偏低");
            
            Debug.Log("请在Unity菜单里面的ILRuntime->Generate CLR Binding Code by Analysis来生成绑定代码");
            
            var type = appdomain.LoadedTypes["HotFix_Project.TestCLRBinding"];
            var m = type.GetMethod("RunTest", 0);
            Debug.Log("请解除InitializeILRuntime方法中的注释对比有无CLR绑定对运行耗时和GC开销的影响");
            sw.Reset();
            sw.Start();
            Profiler.BeginSample("RunTest2");
            appdomain.Invoke(m, null, null);
            Profiler.EndSample();
            sw.Stop();
            Debug.LogFormat("刚刚的方法执行了:{0} ms", sw.ElapsedMilliseconds);

            Debug.Log("可以看到运行时间和GC Alloc有大量的差别，RunTest2之所以有20字节的GC Alloc是因为Editor模式ILRuntime会有调试支持，正式发布（关闭Development Build）时这20字节也会随之消失");
        }
    }

    void RunTest()
    {
        appdomain.Invoke("HotFix_Project.TestCLRBinding", "RunTest", null, null);
    }

    void RunTest2(IMethod m)
    {
        appdomain.Invoke(m, null, null);
    }
}
