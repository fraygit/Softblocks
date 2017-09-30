$(function () {

    window.SoftblockDetail = {
        Render: function (element, appId, formId, cb) {
            //$('#pnlForm').load('/ModuleForm/RenderForm?appId=' + appId + '&reqFormId=' + formId, function () {
            //});
            $(element).load('/ModuleForm/RenderForm?appId=' + appId + '&reqFormId=' + formId, function () {
                cb();
            });
        },

        Init: function () {
            $.each($(".softblock-detail"), function (detailIndex, detailItem) {
                var rawData = $(detailItem).data("softblock-data");

                var components = $(detailItem).find(".detail-component");
                $.each(components, function (componentIndex, componentItem) {
                    var dataType = $(componentItem).data("field-data-type");
                    var fieldName = $(componentItem).data("field-name");
                    switch (dataType) {
                        case "Text":
                            var textHtml = "<strong>" + fieldName + "</strong><br><p class='text-muted'>" + rawData[fieldName] + "</p>";
                            $(componentItem).html(textHtml);
                            break;
                        case "Date":
                            var dateHtml = "<strong>" + fieldName + "</strong><br><p class='text-muted'>" + new Date(rawData["Date Created"].$date) + "</p>";
                            $(componentItem).html(dateHtml);
                            break;
                    }
                });
            });
        }
    }
});