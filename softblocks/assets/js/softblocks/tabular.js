$(function () {

    window.SoftblockTabular = {
        Render: function (element, appId, formId, cb) {
            //$('#pnlForm').load('/ModuleForm/RenderForm?appId=' + appId + '&reqFormId=' + formId, function () {
            //});
            $(element).load('/ModuleForm/RenderForm?appId=' + appId + '&reqFormId=' + formId, function () {
                cb();
            });
        },

        Init: function () {
            $.each($(".softblock-tabular"), function (tabIndex, tabItem) {
                var renderLink = [];
                var dataViewId = $(tabItem).data("data-view-id");
                var appId = $(tabItem).data("app-id");

                var pageDetails = $("#page-details");
                var rootDataId = $(pageDetails).data("data-id");

                var data = $($(tabItem).find("#softblock-data-" + dataViewId)).val();
                //var rawDataObj = JSON.parse(data);

                var documentTypeID = $(tabItem).data("document-type-id");
                var subDocumentTypeID = $(tabItem).data("sub-document-type-id");
                var dataId = $(tabItem).data("data-id");

                var reqParam = {
                    AppId: appId,
                    DocumentTypeId: documentTypeID,
                    DataId: dataId,
                    SubDocumentTypeId: subDocumentTypeID
                };


                var columns = $(tabItem).find(".data-view-columns");
                var colObj = [];
                $.each(columns, function (colIndex, colItem) {
                    var colProp = {
                        FieldName: $(colItem).data("field-name"),
                        DataType: $(colItem).data("field-data-type")
                    }
                    colObj.push(colProp);

                    var isColLink = $(colItem).data("field-is-link").toLowerCase();
                    var isLink = isColLink == 'true';
                    if (isLink) {
                        var pageId = $(colItem).data("field-link-page").toLowerCase();
                        var linkCol = {
                                    "render": function (data, type, row) {
                                        return "<a class='data-table-link' href='/Page?id=" + row[0] + "&moduleId=" + appId + "&pageId=" + pageId + "&rootIds=" + rootDataId + "'>" + row[colIndex + 1] + "</a>";
                                    },
                                    "targets": colIndex + 1
                        }
                        renderLink.push(linkCol);
                    }
                });


                var dataArray = [];

                $.ajax({
                    type: "POST",
                    url: "/DataView/FetchListData",
                    contentType: "application/json",
                    data: JSON.stringify(reqParam),
                    dataType: "json",
                    success: function (d) {
                        var t = d.Result;

                        var rawData = JSON.parse(d.Result);

                        $.each(rawData._v, function (rawDataIndex, rawDataItem) {
                            var arrRow = [];
                            arrRow.push(rawDataItem._id.$oid);
                            $.each(colObj, function (colIndex, colItem) {
                                var value;
                                switch (colItem.DataType) {
                                    case "Date":
                                        eval("value = new Date(rawDataItem['" + colItem.FieldName + "'].$date)");
                                        break;
                                    default:
                                        eval("value = rawDataItem." + colItem.FieldName);
                                        break;
                                }
                                arrRow.push(value);
                            })
                            dataArray.push(arrRow);
                        });

                        renderLink.push({ "visible": false, "targets": [0] }); // hide id column

                        $("#softblock-tabular-" + dataViewId).DataTable({
                            destroy: true,
                            data: dataArray,
                            "columnDefs": renderLink
                        });

                    },
                    error: function (xhr, textStatus, errorThrown) {
                        // TODO: Show error
                    }
                });


                //$.each(rawDataObj, function (rawDataIndex, rawDataItem) {
                //    var arrRow = [];
                //    arrRow.push(rawDataItem._id.$oid);
                //    $.each(colObj, function (colIndex, colItem) {
                //        var value;
                //        switch (colItem.DataType) {
                //            case "Date":
                //                eval("value = new Date(rawDataItem['" + colItem.FieldName + "'].$date)");
                //                break;
                //            default:
                //                eval("value = rawDataItem." + colItem.FieldName);
                //                break;
                //        }
                //        arrRow.push(value);
                //    })
                //    dataArray.push(arrRow);
                //});

                //renderLink.push({"visible": false, "targets": [0]}); // hide id column

                //$("#softblock-tabular-" + dataViewId).DataTable({
                //    destroy: true,
                //    data: dataArray,
                //    "columnDefs": renderLink
                //});

            }); // each tabular panel
        }

    }




});
