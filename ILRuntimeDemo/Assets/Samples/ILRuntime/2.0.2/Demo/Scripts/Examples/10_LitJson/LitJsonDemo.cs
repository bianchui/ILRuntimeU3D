using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;

public class LitJsonDemo : ILRTBase
{
    protected override void InitializeILRuntime()
    {
        base.InitializeILRuntime();
        //这里做一些ILRuntime的注册，这里我们对LitJson进行注册
        LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);
    }

    protected override void OnHotFixLoaded()
    {
        Debug.Log("LitJson在使用前需要初始化，请看InitliazeILRuntime方法中的初始化");
        Debug.Log("LitJson的使用很简单，JsonMapper类里面提供了对象到Json以及Json到对象的转换方法");
        Debug.Log("具体使用方法请看热更项目中的代码");
        //调用无参数静态方法，appdomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        appdomain.Invoke("HotFix_Project.TestJson", "RunTest", null, null);
    }
}
