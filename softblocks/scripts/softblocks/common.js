
function ShowLoading() {
    $("#pnlLoading").show();
}

function HideLoading() {
    $("#pnlLoading").hide();
}


function AddLoadingOverlay(elementName) {
    //$(elementName).block({
    //    message: '<h4><img src="/plugins/images/busy.gif" /> Just a moment...</h4>',
    //    css: {
    //        border: '1px solid #fff'
    //    }
    //});
}

function RemoveLoadingOverlay(elementName) {
    //$(elementName).unblock();
}

function alertSuccess(title, message) {
    toastr.options = {
        "debug": false,
        "newestOnTop": false,
        "positionClass": "toast-top-right",
        "closeButton": true,
        "debug": false,
        "toastClass": "animated fadeInDown"
    };

    toastr.success(message);

}

function alertError(title, message) {
    toastr.options = {
        "debug": false,
        "newestOnTop": false,
        "positionClass": "toast-top-right",
        "closeButton": true,
        "debug": false,
        "toastClass": "animated fadeInDown"
    };

    toastr.error(message);
}

function ConvertObjectId(mongoId) {
    var result =
        pad0(mongoId.Timestamp.toString(16), 8) +
        pad0(mongoId.Machine.toString(16), 6) +
        pad0(mongoId.Pid.toString(16), 4) +
        pad0(mongoId.Increment.toString(16), 6);

    return result;
}

function pad0(str, len) {
    var zeros = "00000000000000000000000000";
    if (str.length < len) {
        return zeros.substr(0, len - str.length) + str;
    }

    return str;
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function RetrieveDocument(jsonBsonDoc, id) {
    var keys = Object.keys(jsonBsonDoc);
    for (var i = 0; i < keys.length; i++) {
        var key = keys[i];
        if (key == "_id") {
            if (jsonBsonDoc[key].$oid == id) {
                return jsonBsonDoc;
            }
        }

        if (jsonBsonDoc[key].constructor == Array) {
            for (var y = 0; y < jsonBsonDoc[key].length; y++) {
                var subDocument = RetrieveDocument(jsonBsonDoc[key][y], id);
                if (subDocument != null) {
                    return subDocument;
                }
            }
        }
    }
    return null;
}

function isBlank(str) {
    return (!str || /^\s*$/.test(str));
}
