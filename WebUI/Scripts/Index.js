var Toolbar;
var Layout;
var Grid;
var Tree;
var Pop;
var Vault;
var SharedTree;
var VaultConf = null;

function init(userName, treeJson, sharedTreeJson)
{
    dhtmlxEvent(window,
        "load",
        function() {

            Layout = new dhtmlXLayoutObject(document.body, "3J"); //2U
            Layout.cells("a").setWidth(250);
            Layout.cells("a").setHeight(300);
            Layout.cells("a").setText("Folders");
            Layout.cells("b").hideHeader();
            Layout.cells("c").setText("Shared folders");

            Menu = new dhtmlXMenuObject();
            Menu.renderAsContextMenu();
            Menu.attachEvent("onClick", onMenuClick);
            Menu.loadStruct("/Content/menuStruct.xml");

            Grid = Layout.cells("b").attachGrid();
            Grid.setImagePath("/Content/icons/");
            Grid.setIconsPath("/Content/imgs/");
            Grid.setHeader("&nbsp;,Name,Type,Modified,&nbsp;");
            Grid.setColTypes("img,ed,ro,ro,img");
            Grid.setInitWidths("70,250,100,*,70");
            Grid.setColAlign("center,left,left,left,center");
            Grid.enableEditEvents(false, false, true);
            Grid.enableContextMenu(Menu);
            Grid.init();
            Grid.attachEvent("onBeforeContextMenu", onShowMenu);

            Tree = Layout.cells("a").attachTree();
            Tree.setImagePath("/Content/imgs/");
            Tree.enableTreeLines(true);
            //Tree.enableCheckBoxes(1);
            //Tree.enableThreeStateCheckboxes(true);
            Tree.enableItemEditor(true);

            SharedTree = Layout.cells("c").attachTree();
            SharedTree.setImagePath("/Content/imgs/");
            SharedTree.enableTreeLines(true);
            SharedTree.enableItemEditor(true);

            Toolbar = Layout.attachToolbar();
            Toolbar.setIconsPath("/Content/icons/");

            window.dhx4.ajax.get('/Home/Configurate', function (r) {
                var t = window.dhx4.s2j(r.xmlDoc.responseText);
                if (t != null) {
                    VaultConf = t;
                    Toolbar.loadStruct("/Content/toolbarStruct.xml", function () {
                        Toolbar.addText("user", 6, "Welcome back, " + userName + "!");
                        Pop = new dhtmlXPopup({ toolbar: Toolbar, id: "addfile" });
                        Pop.attachEvent("onShow", function () {
                            if (Vault == null) {
                                Vault = Pop.attachVault(350, 200, VaultConf);

                                Vault.attachEvent("onUploadComplete", function (files) {
                                    if (dhtmlx.message)
                                        dhtmlx.message("File '" + files.name + "' successfully upload");

                                    loadGrid(Tree.getSelectedItemId());
                                });

                                Vault.attachEvent("onUploadFail", function (file, extra) {
                                    if (dhtmlx.message)
                                        dhtmlx.message("Error while upload " + files.name);
                                });

                                Vault._doUploadFile2 = Vault._doUploadFile;
                                Vault._doUploadFile = function (a) {
                                    this.setURL("/Home/UploadFiles?ID=" + Tree.getSelectedItemId());
                                    this._doUploadFile2.apply(this, arguments);
                                }
                            }
                        });
                    });
                }
            });

            Tree.attachEvent("onClick",
                function (id) {
                    Tree.stopEdit();
                    loadGrid(id);
                    return true;
                });

            SharedTree.attachEvent("onClick",
                function (id) {
                    SharedTree.stopEdit();
                    loadGrid(id);
                    return true;
                });

            var oldTitle;

            Tree.attachEvent("onEdit", function (state, id, tree, value) {
                if (state === 0) {
                    oldTitle = value;
                    return oldTitle;
                }
                if (state === 2) {
                    if (oldTitle !== value) {
                        $.ajax({
                            url: '/Home/EditFolder',
                            type: "POST",
                            dataType: "json",
                            data: { title: value, id: id },
                            success: function () {
                                if (dhtmlx.message)
                                    dhtmlx.message("Folder '" + oldTitle + "' successfully update on '" + value + "'");
                                Tree.setItemText(id, value);
                                return true;
                            },
                            error: function () {
                                if (dhtmlx.message)
                                    dhtmlx.message("Error while updating folder");
                                Tree.stopEdit();
                                return false;
                            }
                        });
                        return true;
                    }
                    return false;
                }
            });

            Grid.attachEvent("onEditCell", function(stage,rId,cInd,nValue,oValue){
                if (stage === 0) {
                    if (Grid.cellById(rId, 2).getValue() === "-")
                        return false;
                    return oValue;
                }
                if (stage === 2) {
                    if (oValue !== nValue) {
                        $.ajax({
                            url: '/Home/EditFile',
                            type: "POST",
                            dataType: "json",
                            data: { id: rId, title: nValue },
                            success: function () {
                                if (dhtmlx.message)
                                    dhtmlx.message("File '" + oValue + "' successfully update on '" + nValue + "'");
                                return true;
                            },
                            error: function () {
                                if (dhtmlx.message)
                                    dhtmlx.message("Error while updating file");
                                return false;
                            }
                        });
                        return true;
                    }
                    return false;
                }
            });

            Grid.attachEvent("onRowDblClicked",
                function (id) {
                    if (Grid.cellById(id, 2).getValue() === "-") {
                        Tree.openItem(id);
                        Tree.selectItem(id, true);
                    }
                    else {
                        showFileContent(id);
                    }
                });

            SharedTree.parse(sharedTreeJson, "json");
            Tree.parse(treeJson ,"json");
            loadGrid(0);
        });    
}


function loadGrid(id) {
    $.ajax({
        url: '/Home/LoadGrid',
        type: "POST",
        dataType: "json",
        data: { id: id },
        success: function (data) {
            Grid.clearAll();
            Grid.parse(JSON.parse(data), "json");
        },
        error: function () {
            if (dhtmlx.message)
                dhtmlx.message("Error while loading grid");
        }
    });
}

function addFolder() {
    var id = Tree.getSelectedItemId();
    var title = Toolbar.getValue("input");
    if (Grid.getSelectedRowId() !== null) {
        if (id === "" && Grid.cellById(Grid.getSelectedRowId(), 2).getValue() === "-")
            id = Grid.getSelectedRowId();
    }

    $.ajax({
        url: '/Home/AddFolder',
        type: "POST",
        dataType: "json",
        data: { title: title, id: id },
        success: function(data) {
            if (dhtmlx.message)
                dhtmlx.message("Folder '" + title + "' successfully created");
            if (id !== "") {
                Tree.insertNewChild(data.parentId, data.id, title, null, "folderClosed.gif");
                Tree.selectItem(data.parentId, true);
            }
            else {
                Tree.deleteChildItems(data.parentId);
                Tree.parse(JSON.parse(data.data), "json");
                Tree.selectItem(data.parentId, true);
            }
        },
        error: function() {
            if (dhtmlx.message)
                dhtmlx.message("Error while creating new folder");
        }
    });
}

function Delete() {
    var id = Tree.getSelectedItemId();
    var title = Tree.getItemText(id);
    var what = "Folder";

    if (Grid.getSelectedRowId() !== null) {
        if (Grid.cellById(Grid.getSelectedRowId(), 2).getValue() === "-") {
            id = Grid.getSelectedRowId();
            title = Grid.cellById(Grid.getSelectedRowId(), 1).getValue();
            what = "Fodler";
        }
        if (Grid.cellById(Grid.getSelectedRowId(), 2).getValue() !== "-") {
            id = Grid.getSelectedRowId();
            title = Grid.cellById(Grid.getSelectedRowId(), 1).getValue();
            what = "File";
        }
    }

    $.ajax({
        url: '/Home/Delete',
        type: "POST",
        dataType: "json",
        data: { id: id ,type : what},
        success: function (data) {
            if (dhtmlx.message)
                dhtmlx.message(what + " '" + title + "' successfully delete");
            if (what === "Folder") {
                Tree.deleteItem(id, false);
                Tree.selectItem(Tree.getParentId(id), true);
            }
            else {
                Grid.clearAll();
                Grid.parse(JSON.parse(data), "json");
            }
        },
        error: function () {
            if (dhtmlx.message)
                dhtmlx.message("Error while deleting " + what);
        }
    });
}

function search() {
    var text = Toolbar.getValue("input");
    $.ajax({
        url: '/Home/Search',
        type: "POST",
        dataType: "json",
        data: { text: text },
        success: function (data) {
            Grid.clearAll();
            Grid.parse(JSON.parse(data), "json");
        },
        error: function () {
            if (dhtmlx.message)
                dhtmlx.message("Nothing found");
        }
    });
}

function logout() {
    $.ajax({
        url: '/Account/LogOut',
        success: function (data) {
            window.location.href = data;
        },
        error: function () {
            if (dhtmlx.message)
                dhtmlx.message("Error");
        }
    });
}

function onMenuClick(menuitemId, type) {
    var id = Grid.contextID.split("_")[0];
    var users = Toolbar.getValue("input");

    switch (menuitemId) {
        case "download":
            if (Grid.cellById(id, 2).getValue() !== "-") {
                window.location.href = "/Home/LoadFile?id=" + id;
            }
            break;
        case "url":
            if (Grid.cellById(id, 2).getValue() !== "-") {
                
            }
            break;
        case "share":
            if (Grid.cellById(id, 2).getValue() === "-") {

                $.ajax({
                    url: '/Home/ShareFolder',
                    type: "POST",
                    dataType: "json",
                    data: { folderId: id, usersLogins: users },
                    success: function () {
                        loadGrid(0);
                    },
                    error: function () {
                        if (dhtmlx.message)
                            dhtmlx.message("Error");
                    }
                });
            }
            break;
        case "unshare":
            if (Grid.cellById(id, 2).getValue() === "-") {
                $.ajax({
                    url: '/Home/UnshareFolder',
                    type: "POST",
                    dataType: "json",
                    data: { folderId: id, usersLogins: users },
                    success: function (data) {
                        loadGrid(0);
                    },
                    error: function () {
                        if (dhtmlx.message)
                            dhtmlx.message("Error");
                    }
                });
            }
            break;
    }
    return true
}

function onShowMenu(rowId, celInd, grid) {
    var arr = ["download", "url", "share", "unshare"];
    for (var i = 0 ; i < arr.length; i++) {
        Menu.hideItem(arr[i]);
    }

    if (Grid.cellById(rowId, 2).getValue() !== "-") {
        Menu.showItem("download");
        Menu.showItem("url");
    }
    else {
        if(Grid.cellById(rowId, 4).getValue().includes("hidden")){
            Menu.showItem("share");
        }
        else{
            Menu.showItem("unshare");
        }
    }

    return true
}

function showFileContent(id) {

    var allowdType = { "text": 1, "image": 1, "video": 1, "audio": 1 };
    var type = Grid.cellById(id, 2).getValue().split('/');

    if (type != null) {

        if (type[0] != null && allowdType[type[0]] == 1) {

            if (!Layout.dhxWins) Layout.dhxWins = new dhtmlXWindows();
            if (!Layout.dhxWins.window("popup")) {
                var w1 = Layout.dhxWins.createWindow("popup", 10000, 10000, 800, 600);
                w1.setText("Preview");
                w1.denyResize();
                w1.button("park").hide();
                w1.button("minmax").hide();
                w1.attachEvent("onClose", function (win) {
                    win.setModal(false);
                    win.hide();
                });
                w1.attachEvent("onContentLoaded", function (win) {
                    win.show();
                    win.setDimension(800, 600);
                    win.center();
                });
            } else {
                w1 = Layout.dhxWins.window("popup");
            }

            if (type[0] === "image") {
                w1.setModal(true);
                var i = document.createElement("IMG");
                var maxWidth = window.screen.availWidth - 30;
                var maxHeight = window.screen.availHeight - 100;
                document.body.appendChild(i);
                i.border = 0;
                i.style.position = "absolute";
                i.style.left = "-2000px";
                i.style.top = "0px";
                i.style.margin = "10px";
                i.onload = function () {
                    w1.attachHTMLString("<div class='image_preview' style='background-image:url(" + this.src + ");'></div>");
                    w1.show();
                    var winWidth = this.offsetWidth;
                    var winHeight = this.offsetHeight;

                    if (this.offsetWidth > maxWidth) {
                        var cof = this.offsetWidth / maxWidth;
                        $("div.image_preview").css(
                            'max-height', this.offsetHeight / cof
                        );
                        winHeight = this.offsetHeight / cof;
                    }
                    if (this.offsetHeight > maxHeight) {
                        var cof = this.offsetHeight / maxHeight;
                        $("div.image_preview").css(
                            'max-width', this.offsetWidth / cof
                        );
                        winWidth = this.offsetWidth / cof;
                    }

                    w1.setDimension(Math.min(winWidth, maxWidth), Math.min(winHeight, maxHeight));
                    w1.center();
                    this.onload = null;
                }
                i.src = "/Home/LoadFIle?id=" + id;
            }

            if (type[0] === "video") {
                w1.setModal(true);
                w1.attachHTMLString("<div class='image_preview'><video width='580' height='500' controls autoplay><source src= /Home/LoadFIle?id=" + id + "></video></div>");
                w1.show();
                w1.setDimension(640, 560);
                w1.center();
            }

            if (type[0] === "audio") {
                w1.setModal(true);
                w1.attachHTMLString("<div><audio controls><source src= /Home/LoadFIle?id=" + id + "></div>");
                w1.show();
                w1.setDimension(300, 50);
                w1.center();
            }

            if (type[0] === "text") {
                w1.setModal(true);
                w1.attachHTMLString("<iframe src= /Home/LoadFIle?id=" + id + " width='500' height='400' ></iframe>");
                w1.show();
                w1.setDimension(508, 400);
                w1.center();
            }

        }
        else {
            if (dhtmlx.message)
                dhtmlx.message("There is no action associated with this file type");
        }
   
    }
}