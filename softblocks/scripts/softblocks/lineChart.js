$(function () {

    window.SoftblockLineChart = {
        Render: function (element, appId, formId, cb) {
            //$('#pnlForm').load('/ModuleForm/RenderForm?appId=' + appId + '&reqFormId=' + formId, function () {
            //});
            $(element).load('/ModuleForm/RenderForm?appId=' + appId + '&reqFormId=' + formId, function () {
                cb();
            });
        },

        Init: function () {
            $.each($(".softblock-line-chart"), function (tabIndex, tabItem) {
                var renderLink = [];
                var dataViewId = $(tabItem).data("data-view-id");
                var appId = $(tabItem).data("app-id");

                var pageDetails = $("#page-details");
                var rootDataId = $(pageDetails).data("data-id");
                var documentName = $(tabItem).data("document-type-name");

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

                var dataLabels = [];

                var fields = $(tabItem).children(".data-view-columns");
                if (fields.length > 1) {

                    var yField = $($(fields)[0]).data("field-name");
                    var xField = $($(fields)[1]).data("field-name");


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
                                if (!isNaN(rawDataItem[yField])) {
                                    if (typeof (rawDataItem[xField]) == "object") {
                                        var dateValue = rawDataItem[xField].$date;
                                        var date = moment(dateValue);
                                        dataLabels.push(moment(dateValue).format("D MMM YY"));
                                    }
                                    else {
                                        dataLabels.push(rawDataItem[xField]);
                                    }
                                    dataArray.push(Number(rawDataItem[yField]));
                                }
                            });

                            var ctx = document.getElementById("softblock-line-chart-" + dataViewId);

                            var myLineChart = new Chart(ctx, {
                                type: 'line',
                                data: {
                                    labels: dataLabels,
                                    datasets: [{
                                        label: yField,
                                        data: dataArray,
                                        borderColor: ["#468d25"],
                                        backgroundColor: ["#62cb31"]
                                    }]
                                }
                            });


                        },
                        error: function (xhr, textStatus, errorThrown) {
                            // TODO: Show error
                        }
                    });
                }

            }); // each tabular panel
        }

    }




});
