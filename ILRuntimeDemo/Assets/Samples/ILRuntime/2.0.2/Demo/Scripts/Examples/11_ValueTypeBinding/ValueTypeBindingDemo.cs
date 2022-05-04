using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;

public class ValueTypeBindingDemo : ILRTBase
{
    protected override void InitializeILRuntime()
    {
        base.InitializeILRuntime();
        //这里做一些ILRuntime的注册，这里我们注册值类型Binder，注释和解注下面的代码来对比性能差别
        appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
        appdomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
        appdomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
    }

    protected override void OnHotFixLoaded()
    {
        StartCoroutine(DoTests());
    }

    IEnumerator DoTests()
    {
        yield return new WaitForSeconds(0.5f);
        RunTest();
        yield return new WaitForSeconds(0.5f);
        RunTest2();
        yield return new WaitForSeconds(0.5f);
        RunTest3();
    }

    void RunTest()
    {
        Debug.Log("Vector3等Unity常用值类型如果不做任何处理，在ILRuntime中使用会产生较多额外的CPU开销和GC Alloc");
        Debug.Log("我们通过值类型绑定可以解决这个问题，只有Unity主工程的值类型才需要此处理，热更DLL内定义的值类型不需要任何处理");        
        Debug.Log("请注释或者解注InitializeILRuntime里的代码来对比进行值类型绑定前后的性能差别");
        //调用无参数静态方法，appdomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        appdomain.Invoke("HotFix_Project.TestValueType", "RunTest", null, null);
    }

    void RunTest2()
    {
        Debug.Log("=======================================");
        Debug.Log("Quaternion测试");
        //调用无参数静态方法，appdomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        appdomain.Invoke("HotFix_Project.TestValueType", "RunTest2", null, null);
    }

    void RunTest3()
    {
        Debug.Log("=======================================");
        Debug.Log("Vector2测试");
        //调用无参数静态方法，appdomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        appdomain.Invoke("HotFix_Project.TestValueType", "RunTest3", null, null);
    }
}
