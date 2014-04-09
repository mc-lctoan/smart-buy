function addCarttoSession(id, name, minPrice, maxPrice) {
    if (typeof (sessionStorage) != "undefined") {
        var exist = false;
        var cart = eval(sessionStorage.cart) || [];
        if (cart.length >= 10) {
            var message = 'Giỏ của bạn không quá 10 sản phẩm.';
            showNotifyDialog(message);            
        }
        else {            
            for (var i = 0; i < cart.length; i++) {
                if (cart[i].id == id) {
                    cart[i].quantity++;
                    exist = true;
                    cart[i].totalmin = Number(cart[i].totalmin) + Number(cart[i].minprice);
                    cart[i].totalmax = Number(cart[i].totalmax) + Number(cart[i].maxprice);
                }

            }
            if (!exist) {
                cart.push({
                    'id': id,
                    'name': name,
                    'minprice': minPrice,
                    'maxprice': maxPrice,
                    'quantity': 1,
                    'totalmin': minPrice,
                    'totalmax': maxPrice
                });
            }
            sessionStorage.cart = JSON.stringify(cart);

            sessionStorage.totalMinPrice = Number(0);
            sessionStorage.totalMaxPrice = Number(0);
            for (var i = 0; i < cart.length; i++) {
                sessionStorage.totalMinPrice = Number(sessionStorage.totalMinPrice) + Number(cart[i].totalmin);
                sessionStorage.totalMaxPrice = Number(sessionStorage.totalMaxPrice) + Number(cart[i].totalmax);
            }

            sessionStorage.items = Number(cart.length);
            var ttp = sessionStorage.items;

            document.getElementById('totalProduct').innerHTML = "( " + ttp + " )";
        }
    } else {
        alert("browser is not support storage!!!");
    }
}

function start(tableID) {

    if (typeof (sessionStorage) != "undefined") {
        if (sessionStorage.cart == null) {
            document.getElementById('totalProduct').innerHTML = "( " + 0 + " )";
        } else {
            document.getElementById('totalProduct').innerHTML = "( " + sessionStorage.items + " )";
        }
    } else {
        alert("browser is not support storage!!!");

    }

}

function showCart(items, tableID) {

    if (typeof (sessionStorage) != "undefined") {
        addRow(tableID, items);
    } else {
        alert("Your browser is not support storage.");
    }

    if (items.length == 0) {
        document.getElementById('btnSaveCart').setAttribute('disabled', 'true');
        document.getElementById('btnSaveCart').style.display = "none";
        document.getElementById('itemCart').deleteRow(1);
        document.getElementById('itemCart').deleteRow(0);
        document.getElementById('saveCartStatus').innerHTML = "(*) Không có sản phẩm nào trong giỏ hàng.";
    }
}

function addRow(tableId, items) {
    var tableElement = document.getElementById(tableId);
    var newCell;
    var col = 0;
    var newRow;

    for (var i = 0; i < items.length; i++) {

        newRow = tableElement.insertRow(tableElement.rows.length);
        newRow.setAttribute("align", "center");

        //STT
        newCell = newRow.insertCell(newRow.cells.length);
        newCell.innerHTML = ++col;

        //product name
        newCell = newRow.insertCell(newRow.cells.length);
        newCell.innerHTML = items[i].name;

        //price        
        newCell = newRow.insertCell(newRow.cells.length);
        newCell.innerHTML = items[i].minprice + " - " + items[i].maxprice;

        //quantity
        newCell = newRow.insertCell(newRow.cells.length);
        newCell.innerHTML = "<div class='input-control text size1'>"
                   + "<input type='text' id='quantityItem" + items[i].id + "'value='" + items[i].quantity
                   + "' onblur=\"updateCart('" + items[i].id + "')\"/></div>";

        //thao tác
        newCell = newRow.insertCell(newRow.cells.length);
        newCell.innerHTML = //"<button onblur=\"updateCart('" + items[i].id + "')\"><i class='icon-checkmark'></i>Cập nhật</button>"
                 " <button onclick=\"removeCartItem('" + items[i].id + "')\"><i class='icon-cancel-2'></i>Xóa</button>";

    }
    newRow = tableElement.insertRow(tableElement.rows.length);

    newCell = newRow.insertCell(newRow.cells.length);
    newCell.setAttribute("colspan", "5");
    newCell.setAttribute("align", "right");
    newCell.innerHTML = "<span>Tổng tiền: " + Math.floor(Number(sessionStorage.totalMinPrice)) + " - " + Math.ceil(Number(sessionStorage.totalMaxPrice)) + " (nghìn đồng)</span>";

    return newRow;
}

function updateCart(id) {
    var el = document.getElementById("quantityItem" + id);
    var quantity = el.value;
    if (validateQuantity(quantity) == false) {
        location.reload();
        return;
    } else {
        if (quantity <= 0) {
            quantity = 0.1;
        } else if (quantity > 10) {
            quantity = 10;
        }
        var cart = eval(sessionStorage.cart);
        for (var i = 0; i < cart.length; i++) {
            if (cart[i].id == id) {
                cart[i].quantity = quantity;
                cart[i].totalmin = cart[i].quantity * cart[i].minprice;
                cart[i].totalmax = cart[i].quantity * cart[i].maxprice;

                sessionStorage.cart = JSON.stringify(cart);

            }
        }

        sessionStorage.totalMinPrice = Number(0);
        sessionStorage.totalMaxPrice = Number(0);
        for (var i = 0; i < cart.length; i++) {
            sessionStorage.totalMinPrice = Number(sessionStorage.totalMinPrice) + Number(cart[i].totalmin);
            sessionStorage.totalMaxPrice = Number(sessionStorage.totalMaxPrice) + Number(cart[i].totalmax);
        }
        location.reload();
    }
}

function removeCartItem(id) {
    var cart = eval(sessionStorage.cart);
    for (var i = 0; i < cart.length; i++) {
        if (cart[i].id == id) {
            cart.splice(i, 1);
            sessionStorage.cart = JSON.stringify(cart);
        }
    }

    sessionStorage.totalMinPrice = Number(0);
    sessionStorage.totalMaxPrice = Number(0);
    for (var j = 0; j < cart.length; j++) {
        sessionStorage.totalMinPrice = Number(sessionStorage.totalMinPrice) + Number(cart[j].totalmin);
        sessionStorage.totalMaxPrice = Number(sessionStorage.totalMaxPrice) + Number(cart[j].totalmax);
    }
    sessionStorage.items = Number(cart.length);
    var ttp = sessionStorage.items;

    document.getElementById('totalProduct').innerHTML = "( " + ttp + " )";

    location.reload();
}

function saveCart() {
    var cart = eval(sessionStorage.cart);
    var data = [];
    
    for (var i = 0; i < cart.length; i++) {
        data[i] = {
            Username: 'Sergey Pimenov',
            ProductId: cart[i].id,
            MinPrice: cart[i].minprice,
            MaxPrice: cart[i].maxprice,
        }
    }
    var totaldata = JSON.stringify(data);
    $.ajax({
        type: 'GET',
        url: 'SaveCart',
        data: { 'totaldata': totaldata },
        contentType: "application/json",
        dataType: 'json',
        success: function (data) {
            if (data == true) {
                clearCartInSession();
                location.href = "../History/BuyingHistory";

            }
            else if (data == 'full') {
                clearCartInSession();
                showNotifyDialog("Giỏ hàng hôm nay đã đầy.");
            }
            else {

                showNotifyDialog("(*) Có lỗi xảy ra. Vui lòng thử lại.");
                var mesDialog = document.getElementById('mesDialog');
                mesDialog.style.color = 'red';
            }
        },
        error: function (e) {            
            showNotifyDialog("(*) Có lỗi xảy ra. Vui lòng thử lại. " + e.message);
            var mesDialog = document.getElementById('mesDialog');
            mesDialog.style.color = 'red';
        }
    })
}

function validateQuantity(quantity) {
    var RE_QUANTITY = /^\d*\.{0,1}\d+$/;
    if (RE_QUANTITY.test(quantity) == true) {
        return true;
    } else return false;
}

function saveUserPrice(productId) {

    var message;
    var marketId = document.getElementById('ddlMarket');
    marketId = marketId.options[marketId.selectedIndex].value;

    var userPrice = document.getElementById('txtUserPrice').value;

    var userPriceData = {        
        MarketId: marketId,
        ProductId: productId,
        UpdatedPrice: userPrice
    }

    var userPriceJson = JSON.stringify(userPriceData);
    validateMarket();
    validateUserPrice();
    if (validateMarket(marketId) == true && validateUserPrice(userPrice) == true) {

        $.ajax({
            type: 'GET',
            url: '../SaveUserPrice',
            data: { 'userPriceJson': userPriceJson },
            contentType: "application/json",
            dataType: 'json',

            success: function (data) {

                if (data == true) {
                    message = "Cảm ơn sự đóng góp của bạn. Chúng tôi sẽ xem xét.";
                    document.getElementById("pppStatus").innerHTML = message;
                    //showNotifyDialog(message);
                    setTimeout(function(){window.parent.location.href = window.parent.location.href},3000);
                    
                } else {
                    message = "(*) Có lỗi xảy ra. Vui lòng thử lại.";
                    showNotifyDialog(message);
                }
            },
            error: function (e) {
                $.Dialog.close();
                message = "(*) Có lỗi xảy ra. Vui lòng thử lại. " + e.message;
                showNotifyDialog(message);
            }
        });
    }

}

function validateUserPrice() {
    var price = document.getElementById('txtUserPrice').value;
    var RE_PRICE = /^\d{1,5}$/;

    if (RE_PRICE.test(price) == false || price == 0 || price > 10000) {
        document.getElementById('errorUserPrice').innerHTML = "Giá không phù hợp. Vui lòng nhập giá trị từ 1 đến 10000";
        return false;
    } else {
        document.getElementById('errorUserPrice').innerHTML = "";
        return true;
    }
}

function validateMarket() {
    var marketId = document.getElementById('ddlMarket');
    marketId = marketId.options[marketId.selectedIndex].value;
    if (trim(marketId) != '') {
        document.getElementById('errorMarket').innerHTML = "";
        return true;
    } else {
        document.getElementById('errorMarket').innerHTML = "Vui lòng chọn địa điểm.";
        return false;
    }
}

function trim(str) {
    var start = 0;
    var end = str.length;
    while (start < str.length && str.charAt(start) == " ")
        start++;
    while (end > 0 && str.charAt(end - 1) == " ")
        end--;
    return str.substr(start, end - start);
}

function clearCartInSession() {
    //remove session cart
    sessionStorage.getItem('cart');
    sessionStorage.removeItem('cart');
    //remove session totalMinPrice
    sessionStorage.getItem('totalMinPrice');
    sessionStorage.removeItem('totalMinPrice');
    //remove session totalMaxPrice
    sessionStorage.getItem('totalMaxPrice');
    sessionStorage.removeItem('totalMaxPrice');
    //remove session items
    sessionStorage.getItem('items');
    sessionStorage.removeItem('items');
}

function showNotifyDialog(mes) {
    $.Dialog({
        shadow: true,
        overlay: true,
        flat: true,
        padding: 10,
        content: '',
        sysButtons: {
            btnClose: true
        },
        sysBtnCloseClick: function (e) {
            window.parent.location.href = window.parent.location.href;
        },
        onShow: function (_dialog) {
            var content = '<div><span id=mesDialog>' + mes + '</span></div><br />'
        + '<div align="center"><button class="button primary" onclick="$.Dialog.close(); window.parent.location.href = window.parent.location.href;">Thoát</button></div> ';
            $.Dialog.content(content);
        }
    });
}

function addToCart(id) {
    $.ajax({
        type: 'GET',
        url: '../Cart/AddToCart',
        data: { 'productId': id },
        contentType: "application/json",
        dataType: 'json',

        success: function (data) {

            if (data == true) {
                //window.parent.location.href = window.parent.location.href;
            } else if (data == 'full') {
                message = "Giỏ của bạn không quá 20 sản phẩm.";
                showNotifyDialog(message);
            }
            else {
                message = "(*) Có lỗi xảy ra. Vui lòng thử lại.";
                showNotifyDialog(message);
            }
        },
        error: function (e) {
            $.Dialog.close();
            message = "(*) Có lỗi xảy ra. Vui lòng thử lại " + e.message;
            showNotifyDialog(message);
        }
    });
    afterbuysuccess(id);

    $('#order-summary').show();
    $("#order-total").load("../Cart/ViewCart #order-total-detail");
}

function afterbuysuccess(id) {
    var offimg = $("#addProduct_"+id).offset();
    var offsum = $('#order-summary').offset();

    var newimg = $("#addProduct_"+id).clone().css({
        'position': 'fixed',
        'top': offimg.top + 'px',
        'left': offimg.left + 'px',
    });
    $('body').append(newimg);
    newimg.animate(
        { 'top': (offsum.top ) + 'px', 'left': (offsum.left +20) + 'px', 'opacity': 1 },
        {
            duration: 1200, complete: function () {
                newimg.remove();
            }
        });

}

function removeFromCart(id) {
    $.ajax({
        type: 'GET',
        url: '../Cart/RemoveFromCart',
        data: { 'productId': id },
        contentType: "application/json",
        dataType: 'json',

        success: function (data) {

            if (data == true) {
                window.parent.location.href = window.parent.location.href;
            } else {
                message = "(*) Có lỗi xảy ra. Vui lòng thử lại.";
                showNotifyDialog(message);
            }
        },
        error: function (e) {
            $.Dialog.close();
            message = "(*) Có lỗi xảy ra. Vui lòng thử lại " + e.message;
            showNotifyDialog(message);
        }
    });
}

function updateQuantityCart(id) {
    var el = document.getElementById("quantityItem " + id);
    var quantity = el.value;
    if (validateQuantity(quantity) == false) {
        window.parent.location.href = window.parent.location.href;
        return;
    } else {
        if (quantity <= 0) {
            quantity = 1;
        } else if (quantity > 10) {
            quantity = 10;
        }
        $.ajax({
            type: 'GET',
            url: '../Cart/UpdateItemInCart',
            data: { 'productId': id, 'quantity': quantity },
            contentType: "application/json",
            dataType: 'json',

            success: function (data) {

                if (data == true) {
                    window.parent.location.href = window.parent.location.href;
                } else {
                    message = "(*) Có lỗi xảy ra. Vui lòng thử lại.";
                    showNotifyDialog(message);
                }
            },
            error: function (e) {
                $.Dialog.close();
                message = "(*) Có lỗi xảy ra. Vui lòng thử lại " + e.message;
                showNotifyDialog(message);
            }
        });
    }
}


