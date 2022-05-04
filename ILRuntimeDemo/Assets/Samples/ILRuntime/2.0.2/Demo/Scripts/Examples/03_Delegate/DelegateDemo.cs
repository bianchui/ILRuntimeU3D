using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public delegate void TestDelegateMethod(int a);
public delegate string TestDelegateFunction(int a);


public class DelegateDemo : ILRTBase
{
    public static TestDelegateMethod TestMethodDelegate;
    public static TestDelegateFunction TestFunctionDelegate;
    public static System.Action<string> TestActionDelegate;
    
    protected override void InitializeILRuntime()
    {
        base.InitializeILRuntime();
        //这里做一些ILRuntime的注册
        //TestDelegateMethod, 这个委托类型为有个参数为int的方法，注册仅需要注册不同的参数搭配即可
        appdomain.DelegateManager.RegisterMethodDelegate<int>();
        //带返回值的委托的话需要用RegisterFunctionDelegate，返回类型为最后一个
        appdomain.DelegateManager.RegisterFunctionDelegate<int, string>();
        //Action<string> 的参数为一个string
        appdomain.DelegateManager.RegisterMethodDelegate<string>();
        
        //ILRuntime内部是用Action和Func这两个系统内置的委托类型来创建实例的，所以其他的委托类型都需要写转换器
        //将Action或者Func转换成目标委托类型

        appdomain.DelegateManager.RegisterDelegateConvertor<TestDelegateMethod>((action) =>
        {
            //转换器的目的是把Action或者Func转换成正确的类型，这里则是把Action<int>转换成TestDelegateMethod
            return new TestDelegateMethod((a) =>
            {
                //调用委托实例
                ((System.Action<int>)action)(a);
            });
        });
        //对于TestDelegateFunction同理，只是是将Func<int, string>转换成TestDelegateFunction
        appdomain.DelegateManager.RegisterDelegateConvertor<TestDelegateFunction>((action) =>
        {
            return new TestDelegateFunction((a) =>
            {
                return ((System.Func<int, string>)action)(a);
            });
        });

        //下面再举一个这个Demo中没有用到，但是UGUI经常遇到的一个委托，例如UnityAction<float>
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<float>>((action) =>
        {
            return new UnityEngine.Events.UnityAction<float>((a) =>
            {
                ((System.Action<float>)action)(a);
            });
        });
    }

    protected override void OnHotFixLoaded()
    {
        Debug.Log("完全在热更DLL内部使用的委托，直接可用，不需要做任何处理");
        
        Debug.Log("如果需要跨域调用委托（将热更DLL里面的委托实例传到Unity主工程用）, 就需要注册适配器");
        Debug.Log("这是因为iOS的IL2CPP模式下，不能动态生成类型，为了避免出现不可预知的问题，我们没有通过反射的方式创建委托实例，因此需要手动进行一些注册");
        Debug.Log("如果没有注册委托适配器，运行时会报错并提示需要的注册代码，直接复制粘贴到ILRuntime初始化的地方");
        appdomain.Invoke("HotFix_Project.TestDelegate", "Initialize2", null, null);
        appdomain.Invoke("HotFix_Project.TestDelegate", "RunTest2", null, null);
        Debug.Log("运行成功，我们可以看见，用Action或者Func当作委托类型的话，可以避免写转换器，所以项目中在不必要的情况下尽量只用Action和Func");
        Debug.Log("另外应该尽量减少不必要的跨域委托调用，如果委托只在热更DLL中用，是不需要进行任何注册的");
        Debug.Log("---------");
        Debug.Log("我们再来在Unity主工程中调用一下刚刚的委托试试");
        TestMethodDelegate(789);
        var str = TestFunctionDelegate(098);
        Debug.Log("!! OnHotFixLoaded str = " + str);
        TestActionDelegate("Hello From Unity Main Project");

    }

    void Update()
    {

    }
}
