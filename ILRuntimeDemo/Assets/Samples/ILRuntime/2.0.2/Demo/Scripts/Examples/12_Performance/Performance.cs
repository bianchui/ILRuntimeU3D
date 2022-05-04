//#define XLUA_INSTALLED
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine.UI;
using ILRuntime.Runtime;
using ILRuntime.Runtime.Enviorment;
#if XLUA_INSTALLED
using XLua;
[LuaCallCSharp]
#endif

public class Performance : ILRTBase
{
    public Button btnLoadStack;
    public Button btnLoadRegister;
    public Button btnUnload;
    public CanvasGroup panelTest;
    public RectTransform panelButton;
    public Text lbResult;
#if XLUA_INSTALLED
    LuaEnv luaenv = null;
    [XLua.CSharpCallLua]
    public delegate void LuaCallPerfCase(StringBuilder sb);
#endif
    List<string> tests = new List<string>();

    private void Awake()
    {
        tests.Add("TestMandelbrot");
        tests.Add("Test0");
        tests.Add("Test1");
        tests.Add("Test2");
        tests.Add("Test3");
        tests.Add("Test4");
        tests.Add("Test5");
        tests.Add("Test6");
        tests.Add("Test7");
        tests.Add("Test8");
        tests.Add("Test9");
        tests.Add("Test10");
        tests.Add("Test11");
        var go = panelButton.GetChild(0).gameObject;
        go.SetActive(false);

        foreach(var i in tests)
        {
            var child = Instantiate(go);
            child.transform.SetParent(panelButton);            
            CreateTestButton(i, child);
            child.SetActive(true);
        }
    }

    private void Start()
    {
        
    }

    void CreateTestButton(string testName, GameObject go)
    {
        Button btn = go.GetComponent<Button>();
        Text txt = go.GetComponentInChildren<Text>();
        txt.text = testName;
        btn.onClick.AddListener(() =>
        {
            StringBuilder sb = new StringBuilder();
#if UNITY_EDITOR || DEBUG
            sb.AppendLine("请打包工程至非Development Build，并安装到真机再测试，编辑器中性能差异巨大，当前测试结果不具备测试意义");
#endif
#if XLUA_INSTALLED
            if (luaenv != null)
            {
                var perf = luaenv.Global.GetInPath<LuaCallPerfCase>(testName);
                perf(sb);
            }
            else
#endif
            appdomain.Invoke("HotFix_Project.TestPerformance", testName, null, sb);
            lbResult.text = sb.ToString();
        });
    }
    public void LoadHotFixAssemblyStack()
    {
        StartHotFixLoad();
    }

    public void LoadHotFixAssemblyRegister()
    {
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        //ILRuntimeJITFlags.JITImmediately表示默认使用寄存器VM执行所有方法
        StartHotFixLoad(ILRuntimeJITFlags.JITImmediately);
    }

    public void LoadLua()
    {
#if XLUA_INSTALLED
        string luaStr = @"require 'performance'";
        luaenv = new LuaEnv();
        luaenv.DoString(luaStr);
#else
        lbResult.text = "请自行安装XLua并生成xlua绑定代码，将performance.lua复制到StreamingAssets后，解除Performace.cs第一行注释";
        Debug.LogError("请自行安装XLua并生成xlua绑定代码后，将performance.lua复制到StreamingAssets后，解除Performace.cs第一行注释");
#endif
        OnHotFixLoaded();
    }

    protected override void InitializeILRuntime()
    {
        base.InitializeILRuntime();
        appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
        appdomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
        appdomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
        ILRuntime.Runtime.CLRBinding.CLRBindingUtils.Initialize(appdomain);
    }

    protected override void OnHotFixLoaded()
    {
        btnUnload.interactable = true;
        panelTest.interactable = true;

    }

    public void Unload()
    {
        UnloadHotFix();
#if XLUA_INSTALLED
        if (luaenv != null)
            luaenv.Dispose();
        luaenv = null;
#endif
        btnUnload.interactable = false;
        btnLoadRegister.interactable = true;
        btnLoadStack.interactable = true;
        panelTest.interactable = false;
    }
    
    void Update()
    {

    }

    public static bool MandelbrotCheck(float workX, float workY)
    {
        return ((workX * workX) + (workY * workY)) < 4.0f;
    }

    public static void TestFunc1(int a, string b, Transform d)
    {
        
    }
}
