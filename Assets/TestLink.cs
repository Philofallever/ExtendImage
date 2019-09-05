using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtendUI.SymbolText;

public class TestLink : MonoBehaviour
{
    private SymbolTextEvent _textEvent;

    protected void Awake()
    {
        _textEvent = GetComponent<SymbolTextEvent>();
        _textEvent.OnClick.AddListener(OnNodeClick);
    }

    protected void OnNodeClick(NodeBase node)
    {
        print(node.GetType().Name);
        if (node is HyperlinkNode hyperlinkNode)
        {
            print($"文本内容:{hyperlinkNode.d_text},超链接:{hyperlinkNode.d_link}");
        }
    }

}
