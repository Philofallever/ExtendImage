//using UnityEngine;
//using System.Collections;
//using SuperScrollView;
//using UnityEngine.UI;

//public class LoopList1 : MonoBehaviour
//{
//    public InputField InputField;
//    public Button SetBtn;
//    public Button ScollTo;
//    public LoopListView2 LoopList;

//    private int dataNum;

//    protected void Awake()
//    {
//        dataNum = 100;
//        LoopList.InitListView(100, OnGetItem1);
//        SetBtn.onClick.AddListener(() =>
//                                   {
//                                       if (int.TryParse(InputField.text, out var num))
//                                       {
//                                           dataNum = num;
//                                           LoopList.SetListItemCount(num,false);
//                                       }
//                                   });

//        ScollTo.onClick.AddListener(() =>
//                                    {
//                                        if (int.TryParse(InputField.text, out var index))
//                                        {
//                                            LoopList.MovePanelToItemIndex(index,0);
//                                        }
//                                    });
//    }

//    private LoopListViewItem2 OnGetItem1(LoopListView2 loopList, int index)
//    {
//        if (index < 0 || index >= dataNum)
//            return null;

//        var data = new  Item1Data()
//                   {
//                       HeadColor = Mathf.CorrelatedColorTemperatureToRGB(40000* index/20f),
//                       Desc =  $"#R这是第{index}个Item的描述,表情啊#{index},哈哈哈哈哈#{index}#{index}#{index}#{index}"
//                   };

//        var item = loopList.NewListViewItem("Item1");

//        if (!item.IsInitHandlerCalled)
//        {
//            var script = item.GetComponent<Item1Script>();
//            item.UserObjectData = script;
//            script.Init();
//        }

//        var item1Script = (Item1Script) item.UserObjectData;
//        item1Script.SetData(data);
//        return item;
//    }
//}
