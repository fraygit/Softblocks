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
                //var rawData = $(detailItem).data("softblock-data");

                var appId = $(detailItem).data("app-id");
                var documentTypeID = $(detailItem).data("document-type-id");
                var subDocumentTypeID = $(detailItem).data("sub-document-type-id");
                var dataId = $(detailItem).data("data-id");
                var parentDataId = window.getParameterByName("rootIds");

                var reqParam = {
                    AppId: appId,
                    DocumentTypeId: documentTypeID,
                    DataId: dataId,
                    DataParentId: parentDataId,
                    SubDocumentTypeId: subDocumentTypeID
                };

                $.ajax({
                    type: "POST",
                    url: "/DataView/FetchData",
                    contentType: "application/json",
                    data: JSON.stringify(reqParam),
                    dataType: "json",
                    success: function (d) {
                        var rawData = JSON.parse(d.Result);

                        var document = RetrieveDocument(rawData, dataId);

                        var components = $(detailItem).find(".detail-component");
                        $.each(components, function (componentIndex, componentItem) {
                            var dataType = $(componentItem).data("field-data-type");
                            var fieldName = $(componentItem).data("field-name");
                            switch (dataType) {
                                case "Text":
                                    var textHtml = "<strong>" + fieldName + "</strong><br><p class='text-muted'>" + document[fieldName] + "</p>";
                                    $(componentItem).html(textHtml);
                                    break;
                                case "Date":
                                    var dateHtml = "<strong>" + fieldName + "</strong><br><p class='text-muted'>" + new Date(document[fieldName].$date) + "</p>";
                                    $(componentItem).html(dateHtml);
                                    break;
                            }
                        });

                    },
                    error: function (xhr, textStatus, errorThrown) {
                        // TODO: Show error
                    }
                });


                //var components = $(detailItem).find(".detail-component");
                //$.each(components, function (componentIndex, componentItem) {
                //    var dataType = $(componentItem).data("field-data-type");
                //    var fieldName = $(componentItem).data("field-name");
                //    switch (dataType) {
                //        case "Text":
                //            var textHtml = "<strong>" + fieldName + "</strong><br><p class='text-muted'>" + rawData[fieldName] + "</p>";
                //            $(componentItem).html(textHtml);
                //            break;
                //        case "Date":
                //            var dateHtml = "<strong>" + fieldName + "</strong><br><p class='text-muted'>" + new Date(rawData["Date Created"].$date) + "</p>";
                //            $(componentItem).html(dateHtml);
                //            break;
                //    }
                //});
            });
        }
    }
});