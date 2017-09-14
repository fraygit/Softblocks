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