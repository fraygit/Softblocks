$(function () {

    var SoftblockPage = {
        RenderForm: function (element, appId, formId) {
            //$('#pnlForm').load('/ModuleForm/RenderForm?appId=' + appId + '&reqFormId=' + formId, function () {
            //});
            $(element).load('/ModuleForm/RenderForm?appId=' + appId + '&reqFormId=' + formId, function () {
            });

        }
    };

    $.each($(".page-panel"), function (panelIndex, panelItem) {
        var panelType = $(panelItem).data("panel-type");
        var foreignId = $(panelItem).data("foreign-id");
        var appId = $(panelItem).data("app-id");
        switch (panelType) {
            case "Forms":
                SoftblockPage.RenderForm(panelItem, appId, foreignId);
                break;
        }
    });

});
