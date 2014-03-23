var displayType, currentStep;
var prevExp = "";
var $tile = $('#xpathTile');
var $productName = $('#xpathProductName');
var $price = $('#xpathPrice');
var $paging = $('#xpathPaging');

$('#ParserForm').wizard({
    nextButtonLabel: "Sau &raquo;",
    prevButtonLabel: "&laquo; Trước",
    submitButtonLabel: "Hoàn thành",
    onStepLeave: validateStep,
    onStepShown: showStep
});

$('form#ParserForm button[class="btn btn-primary pull-right"]').click(function (event) {
    event.stopImmediatePropagation();
    location.href = "/Admin/Parser";
});

function validateStep(wzr, fset) {
    currentStep = getStep(wzr._activeStepId);
    if (typeof fset != "undefined") {
        switch (fset.dataset["name"]) {
            case "type":
                displayType = $('input[type="radio"]:checked', fset).val();
                if (displayType == "table") {
                    $tile.prop("disabled", true);
                } else {
                    $tile.prop("disabled", false);
                }
                return true;
            case "divInfo":
                if (displayType == "grid" && $tile.val() == "") {
                    alert("Phải chọn khung thông tin");
                    return false;
                }
                return true;
            case "productName":
                if ($productName.val() == "") {
                    alert("Phải chọn tên sản phẩm");
                    return false;
                }
                return true;
            case "productPrice":
                if ($price.val() == "") {
                    alert("Phải chọn giá sản phẩm");
                    return false;
                }
                return true;
            case "paging":
                return true;
            default:
                return true;
        }
    }
    return true;
}

function showStep(wzr) {
    var step = getStep(wzr._activeStepId);
    //if (displayType == "table" && step == "1") {
    //    if (currentStep < step) {
    //        step++;
    //        wzr.nextStep();
    //    } else {
    //        step--;
    //        wzr.prevStep();
    //    }
    //}
    currentStep = step;
}

function getStep(str) {
    if (str == null) {
        return 0;
    }
    var last = str.length - 1;
    return Number(str[last]);
}

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

function clearHighlight(xpathExpression) {
    var selected = myFrameDoc.evaluate(xpathExpression, myFrameDoc, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
    for (var i = 0; i < selected.snapshotLength; i++) {
        var node = selected.snapshotItem(i);
        node.style.outline = "none";
    }
}

function highlightElement(xpathExpression) {
    xpathExpression = xpathExpression.replace("[i]", "");
    if (prevExp != "") {
        clearHighlight(prevExp);
    }
    var selected = myFrameDoc.evaluate(xpathExpression, myFrameDoc, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
    for (var i = 0; i < selected.snapshotLength; i++) {
        var node = selected.snapshotItem(i);
        node.style.outline = "thin dashed #FF0000";
    }
    prevExp = xpathExpression;
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
    if (currentStep == 2) {
        $productName.val(expression);
    } else if (currentStep == 3) {
        $price.val(expression);
    } else if (currentStep == 1) {
        $tile.val(expression);
    } else if (currentStep == 4) {
        $paging.val(expression);
    }
}

function getXPath(event) {
    event.preventDefault();
    if (currentStep == 4) {
        getPaging(event);
        return;
    }
    if (displayType == "table" && currentStep != 1) {
        getTabularPath(event);
    } else if (displayType == "grid") {
        getGridPath(event);
    }
}

function getTabularPath(event) {
    var xpath = getPath(event.target, webDiv);
    var result = "";
    for (var i = 0; i < xpath.length; i++) {
        if (xpath[i].tagName == "tr") {
            xpath[i].position = -1;
            xpath[i].tagName += "[i]";
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
    //document.getElementById("rad_tile").checked
    if (currentStep == 1) {
        tile = target;
        result += "//" + target.tagName.toLowerCase() + "[@class='" + target.className + "']";
        setTextBoxXpathValue(result);
        highlightElement(result);
        return;
    }

    if ($tile.val() == "") {
        alert("Phải chọn khung thông tin trước!");
        return;
    }
    result = document.getElementById("xpathTile").value;
    result += "[i]";
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

function getPaging(event) {
    var xpath = getPath(event.target, webDiv);
    xpath[0].position = -1;
    //xpath[0].tagName += "[i]";
    xpath.reverse();
    var result = "";

    if (xpath[0].id != "") {
        result += "//*[@id='" + xpath[0].id + "']";
        for (var i = 1; i < xpath.length; i++) {
            result += "/" + xpath[i].tagName;
            if (xpath[i].position != -1) {
                result += "[" + xpath[i].position + "]";
            }
        }
    } else {
        for (i = 0; i < xpath.length; i++) {
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