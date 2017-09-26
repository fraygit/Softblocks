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
                var dataViewId = $(tabItem).data("data-view-id");

                var columns = $(tabItem).find(".data-view-columns");
                var colObj = [];
                $.each(columns, function (colIndex, colItem) {
                    var colProp = {
                        FieldName: $(colItem).data("field-name"),
                        DataType: $(colItem).data("field-data-type")
                    }
                    colObj.push(colProp);
                });

                var data = $($(tabItem).find("#softblock-data-" + dataViewId)).val();
                var rawDataObj = JSON.parse(data);

                var dataArray = [];

                $.each(rawDataObj, function (rawDataIndex, rawDataItem) {
                    var arrRow = [];
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

                $("#softblock-tabular-" + dataViewId).DataTable({
                    data: dataArray
                });

            }); // each tabular panel
        }

    }




});
