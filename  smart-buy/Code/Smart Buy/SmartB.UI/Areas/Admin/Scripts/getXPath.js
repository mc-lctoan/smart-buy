var tmp = document.getElementById("webDiv");
var myFrameDoc;
tmp.onload = init;
var webDiv = null;
var tile = null;

function init() {
    webDiv = (tmp.contentWindow || tmp.contentDocument);
    if (webDiv.document) {
        myFrameDoc = webDiv.document;
        webDiv = webDiv.document.body;
    }
    //tmp.width = webDiv.scrollWidth;
    if (webDiv != null) {
        webDiv.onclick = getXPath;
        var divChild = webDiv.childNodes;
        for (var i = 0; i < divChild.length; i++) {
            divChild[i].onmouseover = mouseIn;
            divChild[i].onmouseout = mouseOut;
        }
    }
}

function mouseIn(event) {
    var element = event.target;
    element.style.outline = "thin dashed #FF0000";
}

function mouseOut(event) {
    var element = event.target;
    element.style.outline = "none";
}

function highlightElement(xpathExpression) {
    var selected = myFrameDoc.evaluate(xpathExpression, myFrameDoc, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
    for (var i = 0; i < selected.snapshotLength; i++) {
        var node = selected.snapshotItem(i);
        node.style.outline = "thin dashed #FF0000";
    }
}

function XpathElement(tagName, id, className, position) {
    this.tagName = tagName;
    this.id = id;
    this.className = className;
    this.position = position;
}

function getPath(clickedNode, root) {
    var node = clickedNode;

    var parent = null;
    var children = null;
    var element = null;
    var pos = 0;
    var xpath = [];
    var counter = 0;
    var foundId = false;
    while (true) {
        if (node === root) {
            break;
        }
        if (node.id !== "") {
            foundId = true;
            break;
        }
        parent = node.parentNode;
        children = parent.childNodes;
        counter = 0;
        pos = 0;
        for (var i = 0; i < children.length; i++) {
            if (children[i].nodeType == 1 && children[i].tagName == node.tagName) {
                counter++;
            }
            if (children[i] == node) {
                pos = counter;
            }
            if (pos > 0 && counter > 1) {
                break;
            }
        }
        element = new XpathElement(node.tagName.toLowerCase(), node.id, node.className, -1);
        if (counter > 1) {
            element.position = pos;
        }
        xpath.push(element);
        node = parent;
    }
    if (foundId) {
        element = new XpathElement(node.tagName.toLowerCase(), node.id, node.className, -1);
        xpath.push(element);
    } else if (root == webDiv) {
        element = new XpathElement("body", node.id, node.className, -1);
        xpath.push(element);
        element = new XpathElement("html", node.id, node.className, -1);
        xpath.push(element);
    }
    return xpath;
}

function setTextBoxXpathValue(expression) {
    if (document.getElementById("rad_productName").checked) {
        document.getElementById("xpathProductName").value = expression;
    } else if (document.getElementById("rad_price").checked) {
        document.getElementById("xpathPrice").value = expression;
    } else if (document.getElementById("rad_tile").checked) {
        document.getElementById("xpathTile").value = expression;
    }
}

function getXPath(event) {
    event.preventDefault();
    if (document.getElementById("rad_tabularView").checked) {
        getTabularPath(event);
    } else if (document.getElementById("rad_gridView").checked) {
        getGridPath(event);
    }
}

function getTabularPath(event) {
    var xpath = getPath(event.target, webDiv);
    var result = "";
    for (var i = 0; i < xpath.length; i++) {
        if (xpath[i].tagName == "tr") {
            xpath[i].position = -1;
            break;
        }
    }
    xpath.reverse();
    if (xpath[0].id != "") {
        result += "//*[@id='" + xpath[0].id + "']";
        for (i = 1; i < xpath.length; i++) {
            result += "/" + xpath[i].tagName;
            if (xpath[i].position != -1) {
                result += "[" + xpath[i].position + "]";
            }
        }
    } else {
        for (i = 0; i < xpath.length; i++) {
            result += xpath[i].tagName;
            if (xpath[i].position != -1) {
                result += "[" + xpath[i].position + "]";
            }
            result += "/";
        }
        result = result.slice(0, -1);
    }
    highlightElement(result);
    result = result.replace("/tbody", "");
    setTextBoxXpathValue(result);
}

function getGridPath(event) {
    var target = event.target;
    var result = "";
    if (document.getElementById("rad_tile").checked) {
        tile = target;
        result += "//" + target.tagName.toLowerCase() + "[@class='" + target.className + "']";
        setTextBoxXpathValue(result);
        highlightElement(result);
        return;
    }

    if (tile == null) {
        alert("Phải chọn khung thông tin trước!");
        return;
    }
    result = document.getElementById("xpathTile").value;
    var xpath = getPath(target, tile);
    xpath.reverse();
    for (var i = 0; i < xpath.length; i++) {
        result += "/" + xpath[i].tagName;
        if (xpath[i].position != -1) {
            result += "[" + xpath[i].position + "]";
        }
    }
    setTextBoxXpathValue(result);
    highlightElement(result);
}