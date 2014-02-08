function addCarttoSession(id, name, minPrice, maxPrice) {
    if (typeof (sessionStorage) != "undefined") {


        var exist = false;
        var cart = eval(sessionStorage.cart) || [];
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

        //sessionStorage.totalPrice = Number(0);
        //for (var i = 0; i < cart.length; i++) {
        //    sessionStorage.totalPrice = Number(sessionStorage.totalPrice) + Number(cart[i].total);
        //}
        //var ttp = sessionStorage.totalPrice;

        //document.getElementById('totalPrice').innerHTML = Number(ttp).formatMoney(0);

        sessionStorage.items = Number(cart.length);        
        var ttp = sessionStorage.items;

        document.getElementById('totalProduct').innerHTML = "( " + ttp + " )";

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
    var items = eval(sessionStorage.cart);
    showCart(items, tableID);
}

function showCart(items, tableID) {

    if (typeof (sessionStorage) != "undefined") {
        addRow(tableID, items);
    } else {
        alert("Your browser is not support storage.");

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
                   + "<input type='number' value='" + items[i].quantity + "'min='0' max='10' step='0.1'/></div>";            

        //thao tác
        newCell = newRow.insertCell(newRow.cells.length);
        newCell.innerHTML = "<button><i class='icon-checkmark'></i>Cập nhật</button>"
                +"<button><i class='icon-cancel-2'></i>Xóa</button>";

    }
    newRow = tableElement.insertRow(tableElement.rows.length);

    //newCell = newRow.insertCell(newRow.cells.length);
    //newCell.setAttribute("colspan", "6");
    //newCell.setAttribute("align", "right");
    //newCell.innerHTML = "<span class='allPrice'>Thành tiền: " + Number(sessionStorage.totalPrice) + "</span>";

    return newRow;
}
