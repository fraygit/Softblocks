function AddLoadingOverlay(elementName) {
    $(elementName).block({
        message: '<h4><img src="/plugins/images/busy.gif" /> Just a moment...</h4>',
        css: {
            border: '1px solid #fff'
        }
    });
}

function RemoveLoadingOverlay(elementName) {
    $(elementName).unblock();
}

function alertSuccess(title, message) {
    $.toast({
        heading: title,
        text: message,
        position: 'top-right',
        loaderBg: '#ff6849',
        icon: 'success',
        hideAfter: 2000,
        stack: 6
    });
}

function alertError(title, message) {
    $.toast({
        heading: title,
        text: message,
        position: 'top-right',
        loaderBg: '#ff6849',
        icon: 'error',
        hideAfter: 2000,
        stack: 6
    });
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