var tmp = document.getElementById("webDiv");
var myFrameDoc;
tmp.onload = init;
var webDiv = null;

function init() {
    webDiv = (tmp.contentWindow || tmp.contentDocument);
    if (webDiv.document) {
        myFrameDoc = webDiv.document;
        webDiv = webDiv.document.body;
    }
    tmp.width = webDiv.scrollWidth;
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

function getPath(event) {
    var node = event.target;

    var parent = null;
    var children = null;
    var element = null;
    var pos = 0;
    var xpath = [];
    //var tmp = "";
    var counter = 0;
    var foundId = false;
    while (true) {
        //tmp = "";
        if (node === webDiv) {
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
        //tmp += node.tagName.toLowerCase();
        element = new XpathElement(node.tagName.toLowerCase(), node.id, node.className, -1);
        if (counter > 1) {
            //tmp += "[" + pos + "]";
            element.position = pos;
        }
        //xpath.push(tmp);
        xpath.push(element);
        node = parent;
    }
    if (foundId) {
        element = new XpathElement(node.tagName.toLowerCase(), node.id, node.className, -1);
        xpath.push(element);
        //xpath.push("//*" + "[@id='" + node.id + "']");
    } else {
        element = new XpathElement("body", node.id, node.className, -1);
        xpath.push(element);
        element = new XpathElement("html", node.id, node.className, -1);
        xpath.push(element);
    }
    return xpath;
    //xpath.reverse();
    //var result = xpath.join("/");
    //document.getElementById("chosenPath").innerHTML = result;
    //alert(result);
}

function setTextBoxXpathValue(expression) {
    if (document.getElementById("rad_productName").checked) {
        document.getElementById("xpathProductName").value = expression;
    } else if (document.getElementById("rad_price").checked) {
        document.getElementById("xpathPrice").value = expression;
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
    var xpath = getPath(event);
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
    setTextBoxXpathValue(result);
    highlightElement(result);
}

function getGridPath(event) {
    var target = event.target;
    //if (target.className == "") {
    //    alert("Cannot get XPath for this element!");
    //    return;
    //}
    var xpath = getPath(event);
    var parent = target.parentNode;
    var children = parent.childNodes;
    var counter = 0;
    var pos = 0;
    for (var i = 0; i < children.length; i++) {
        if (children[i].nodeType == 1 && children[i].tagName == target.tagName && children[i].className == target.className) {
            counter++;
        }
        if (children[i] == target) {
            pos = counter;
        }
        if (pos > 0 && counter > 1) {
            break;
        }
    }
    var posExpression = "";
    if (counter > 1) {
        posExpression = "[" + pos + "]";
    }

    var newXpathArray = null;
    for (i = 1; i < xpath.length; i++) {
        if (xpath[i].className != "") {
            newXpathArray = xpath.slice(0, i + 1);
            break;
        }
    }
    if (newXpathArray != null) {
        newXpathArray.reverse();
        var result = "//" + newXpathArray[0].tagName + "[@class='" + newXpathArray[0].className + "']";
        for (i = 1; i < newXpathArray.length - 1; i++) {
            result += "/" + newXpathArray[i].tagName + "[" + newXpathArray[i].position +"]";
        }
        var tmp = newXpathArray.length - 1;
        result += "/" + newXpathArray[tmp].tagName;
        if (newXpathArray[tmp].className != "") {
            result += "[@class='" + newXpathArray[tmp].className + "']";
        }
        result += posExpression;
        
        setTextBoxXpathValue(result);
        highlightElement(result);
    }
}