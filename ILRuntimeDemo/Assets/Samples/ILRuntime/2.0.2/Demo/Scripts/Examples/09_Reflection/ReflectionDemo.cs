using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class ReflectionDemo : ILRTBase
{
    protected override void InitializeILRuntime()
    {
        base.InitializeILRuntime();
        //这里做一些ILRuntime的注册，比如委托的适配器，但是为了演示不些适配器的报错，注册写在了OnHotFixLoaded里

    }

    protected override void OnHotFixLoaded()
    {
        Debug.Log("C#工程中反射是一个非常经常用到功能，ILRuntime也对反射进行了支持，在热更DLL中使用反射跟原生C#没有任何区别，故不做介绍");
        Debug.Log("这个Demo主要是介绍如何在主工程中反射热更DLL中的类型");
        Debug.Log("假设我们要通过反射创建HotFix_Project.InstanceClass的实例");
        Debug.Log("显然我们通过Activator或者Type.GetType(\"HotFix_Project.InstanceClass\")是无法取到类型信息的");
        Debug.Log("热更DLL中的类型我们均需要通过AppDomain取得");
        var it = appdomain.LoadedTypes["HotFix_Project.InstanceClass"];
        Debug.Log("LoadedTypes返回的是IType类型，但是我们需要获得对应的System.Type才能继续使用反射接口");
        var type = it.ReflectionType;
        Debug.Log("取得Type之后就可以按照我们熟悉的方式来反射调用了");
        var ctor = type.GetConstructor(new System.Type[0]);
        var obj = ctor.Invoke(null);
        Debug.Log("打印一下结果");
        Debug.Log(obj);
        Debug.Log("我们试一下用反射给字段赋值");
        var fi = type.GetField("id", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        fi.SetValue(obj, 111111);
        Debug.Log("我们用反射调用属性检查刚刚的赋值");
        var pi = type.GetProperty("ID");
        Debug.Log("ID = " + pi.GetValue(obj, null));
    }
}
