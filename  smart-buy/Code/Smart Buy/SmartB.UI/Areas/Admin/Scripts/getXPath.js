var tmp = document.getElementById("webDiv");
tmp.onload = init;
var webDiv = null;

function init() {
    webDiv = (tmp.contentWindow || tmp.contentDocument);
    if (webDiv.document)webDiv = webDiv.document.body;
    tmp.width = webDiv.scrollWidth;
    if (webDiv != null) {
        webDiv.onclick = getPath;
        var divChild = webDiv.childNodes;
        for (i = 0; i < divChild.length; i++) {
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

function getPath(event) {
    var node = event.target;
    event.preventDefault();

    var parent = null;
    var children = null;
    var pos = 0;
    var xpath = [];
    var tmp = "";
    var counter = 0;
    var foundId = false;
    while (true) {
        tmp = "";
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
        for (i = 0; i < children.length; i++) {
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
        tmp += node.tagName;
        if (counter > 1) {
            tmp += "[" + pos + "]";
        }
        xpath.push(tmp);
        node = parent;
    }
    if (foundId) {
        xpath.push("//*" + "[@id='" + node.id + "']");
    } else {
        xpath.push("body");
        xpath.push("html");
    }
    xpath.reverse();
    var result = xpath.join("/").toLowerCase();
    alert(result);
}