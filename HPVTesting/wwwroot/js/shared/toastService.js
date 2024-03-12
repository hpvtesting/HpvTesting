/**
 * Class displaying toast messages
 */
var ToastService = function () {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "positionClass": "toast-bottom-left",
        "onclick": null,
        "showDuration": "300000",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
};

/**
 * Show a success message.
 */
ToastService.prototype.success = function (message, description) {
    toastr.success(description, message);
};

/**
 * Show an error message.
 */
ToastService.prototype.error = function (message, description) {
    toastr.error(description, message);
};

/**
 * Show an info message.
 */
ToastService.prototype.info = function (message, description) {
    toastr.info(description, message);
};

/**
 * Show a warning message.
 */
ToastService.prototype.warning = function (message, description) {
    toastr.warning(description, message);
};

ToastService.prototype.displayDropMessage = function () {
    var messageCookie = $.cookie("DropMessage");
    if (messageCookie == null) {
        return;
    }

    var message = JSON.parse(messageCookie);

    switch (message.DropMessageType) {
        case 0:
            this.success(message.Message, message.Description);
            break;

        case 1:
            this.error(message.Message, message.Description);
            break;

        case 2:
            this.warning(message.Message, message.Description);
            break;

        case 3:
            this.info(message.Message, message.Description);
            break;
    }

    $.removeCookie('DropMessage', { path: '/' });
};