using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntimeDemo;

public abstract class TestClassBase
{
    public virtual int Value
    {
        get
        {
            return 0;
        }
        set
        {

        }
    }

    public virtual void TestVirtual(string str)
    {
        Debug.Log("!! TestClassBase.TestVirtual, str = " + str);
    }

    public abstract void TestAbstract(int gg);
}
public class Inheritance : ILRTBase
{
    protected override void InitializeILRuntime()
    {
        base.InitializeILRuntime();
        //这里做一些ILRuntime的注册，这里应该写继承适配器的注册，为了演示方便，这个例子写在OnHotFixLoaded了
    }

    protected override void OnHotFixLoaded()
    {
        Debug.Log("首先我们来创建热更里的类实例");
        TestClassBase obj;
        Debug.Log("现在我们来注册适配器, 该适配器由ILRuntime/Generate Cross Binding Adapter菜单命令自动生成");
        appdomain.RegisterCrossBindingAdaptor(new TestClassBaseAdapter());
        Debug.Log("现在再来尝试创建一个实例");
        obj = appdomain.Instantiate<TestClassBase>("HotFix_Project.TestInheritance");
        Debug.Log("现在来调用成员方法");
        obj.TestAbstract(123);
        obj.TestVirtual("Hello");
        obj.Value = 233;
        Debug.LogFormat("obj.Value={0}", obj.Value);


        Debug.Log("现在换个方式创建实例");
        obj = appdomain.Invoke("HotFix_Project.TestInheritance", "NewObject", null, null) as TestClassBase;
        obj.TestAbstract(456);
        obj.TestVirtual("Foobar");
        obj.Value = 2333333;
        Debug.LogFormat("obj.Value={0}", obj.Value);
    }

    void Update()
    {

    }
}
