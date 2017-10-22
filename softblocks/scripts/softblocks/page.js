$(function () {

    var SoftblockPage = {
        RenderForm: function (element, appId, formId, cb) {
            $(element).load('/ModuleForm/RenderForm?appId=' + appId + '&reqFormId=' + formId, function () {
                cb();
            });
        },

        RenderDataView: function (element, appId, dataViewId, isPreview, cb) {
            var pageDetails = $("#page-details");
            var dataId = $(pageDetails).data("data-id");

            $(element).load('/DataView/RenderDataView?appId=' + appId + '&id=' + dataViewId + '&isPreview=' + isPreview + '&dataId=' + dataId, function () {
                cb();
            });
        },

        ExtractFormValues: function (formId) {
            var formFieldElemets = $("#frm-" + formId + " .form-field");
            var dataString = '';
            $.each(formFieldElemets, function (formFieldIndex, formFieldItem) {
                var fieldName = $(formFieldItem).data("field-name");
                var dataType = $(formFieldItem).data("field-data-type");

                var value = '';
                switch (dataType) {
                    case "Text":
                        value = $(formFieldItem).val();
                        dataString += '"' + fieldName + '": ' + '"' + value + '"';
                        break;
                    case "Date":
                        var dateValue = $(formFieldItem).datepicker('getDate');
                        dataString += '"' + fieldName + '": ISODate("' + dateValue.toISOString() + '")';
                        break;
                    case "Integer":
                        value = $(formFieldItem).val();
                        dataString += '"' + fieldName + '": ' + '"' + value + '"';
                        break;
                }

                if (dataString.length > 0) {
                    dataString += ", ";
                }
                

            });

            //dataString += ',"TestNum": ' + '13, ';
            //dataString += '"TestDate": ISODate("' + new Date().toISOString() + '")';
            dataString = "{" + dataString + "}";

            return dataString;
        },

        SubmitEvent: function (appId, foreignId, panelId) {
            $.each($(".form-submit-btn"), function (btnIndex, btnItem) {

                var pageDetails = $("#page-details");
                var rootDataId = $(pageDetails).data("data-id");
                var parentDocumentName = $(pageDetails).data("parent-document-name");

                $(btnItem).click(function () {

                    var formDataValues = SoftblockPage.ExtractFormValues(foreignId);
                    var reqParam = {
                        appId : appId,
                        foreignId: foreignId,
                        data: formDataValues,
                        RootDataId: rootDataId,
                        ParentDocumentName: parentDocumentName
                    }

                    $.ajax({
                        type: "POST",
                        url: "/ModuleForm/Insert?appId=" + appId + '&reqFormId=' + foreignId,
                        contentType: "application/json",
                        data: JSON.stringify(reqParam),
                        dataType: "json",
                        success: function (d) {
                            if (d.IsSuccess == true) {
                                alertSuccess("Success", "Successfully submitted!");
                                window.RenderPagePanels();
                            }
                            else {
                                alertError("Error!", d.Message);
                            }
                        },
                        error: function (xhr, textStatus, errorThrown) {
                            // TODO: Show error
                        }
                    });
                });
            });
        }
    };


    window.RenderPagePanels = function () {
        $.each($(".page-panel"), function (panelIndex, panelItem) {

            var panelType = $(panelItem).data("panel-type");
            var foreignId = $(panelItem).data("foreign-id");
            var appId = $(panelItem).data("app-id");
            var panelId = $(panelItem).data("panel-Id");
            var isPreview = $(panelItem).data("is-preview");
            switch (panelType) {
                case "Forms":
                    SoftblockPage.RenderForm(panelItem, appId, foreignId, function () {
                        SoftblockPage.SubmitEvent(appId, foreignId, panelId);

                        //DATE PICKER
                        $.each($(".field-date"), function (dateElIndex, dateElItem) {
                            $(dateElItem).datepicker();
                        });
                    });
                    break;
                case "Data View":
                    SoftblockPage.RenderDataView(panelItem, appId, foreignId, isPreview, function () {
                        window.SoftblockTabular.Init();
                        window.SoftblockDetail.Init();
                        window.SoftblockLineChart.Init();
                    });
                    break;
            }
        });
    }
    window.RenderPagePanels();




});
