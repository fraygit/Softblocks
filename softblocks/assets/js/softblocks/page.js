$(function () {

    var SoftblockPage = {
        RenderForm: function (element, appId, formId, cb) {
            //$('#pnlForm').load('/ModuleForm/RenderForm?appId=' + appId + '&reqFormId=' + formId, function () {
            //});
            $(element).load('/ModuleForm/RenderForm?appId=' + appId + '&reqFormId=' + formId, function () {
                cb();
            });
        },

        SubmitEvent: function (appId, foreignId) {
            $.each($(".form-submit-btn"), function (btnIndex, btnItem) {
                $(btnItem).click(function () {
                    $.ajax({
                        type: "GET",
                        url: "/ModuleForm/Insert?appId=" + appId + '&reqFormId=' + foreignId,
                        contentType: "application/json",
                        dataType: "json",
                        success: function (d) {
                            if (d.IsSuccess == true) {
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

    $.each($(".page-panel"), function (panelIndex, panelItem) {
        var panelType = $(panelItem).data("panel-type");
        var foreignId = $(panelItem).data("foreign-id");
        var appId = $(panelItem).data("app-id");
        switch (panelType) {
            case "Forms":
                SoftblockPage.RenderForm(panelItem, appId, foreignId, function () {
                    SoftblockPage.SubmitEvent(appId, foreignId);
                });
                break;
        }
    });



});
