(function ($) {
    var configuration = {
        'default': {
            type: "POST",
            cache: false,
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=utf-8'
        },
        'object': {
            contentType: 'application/json; charset=utf-8'
        },
        'formData': {
            contentType: false, // do not add a Content-Type header, the boundary string will be missing from it
            processData: false // do not process into string
        }
    };

    var getType = function (data) {
        if (data instanceof FormData)
            return 'formData';

        if (typeof data == 'object')
            return 'object';

        if ($.isNumeric(data))
            return 'id';

        return 'urlencoded'; // default to a serialized object
    };

    var prepareData = function (type, data) {
        switch (type) {
            case 'object':
                return JSON.stringify(data);
            case 'id':
                return 'id=' + data;
            default:
                return data;
        }
    };

    $.ajaxAction = function (url, data) {
        var deferred = $.Deferred();
        var dataType = getType(data);
        var parameters = $.extend({},
            configuration.default,
            configuration[dataType] || {},
            {
                url: url,
                data: prepareData(dataType, data)
            });

        $.loading(true);

        $.ajax(parameters)
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.loading(false);
                // User is navigating away from page
                if (jqXHR.status == 0)
                    return;

                // 401 Unauthorized - Session expired
                if (jqXHR.status === 401) {
                    window.sessionExpiredError();
                    return;
                }

                var resultData;

                try {
                    resultData = $.parseJSON(jqXHR.responseText);
                } catch (e) {
                    //window.fatalError("Invalid JSON response", "Action: " + url);
                    IsSessionExists();
                    return;
                }

                if (resultData.Result != "PostJsonResult") {
                    alert("I can only handle PostJsonResult objects!");
                    return;
                }

                if (jqXHR.status == 500) {
                    toastr.error(resultData.Message, "Exception");
                }

                if (resultData.DropMessage) {
                    var messageTypes = ["success", "error", "warning", "info"];
                    toastr[messageTypes[resultData.DropMessage
                        .DropMessageType]](resultData.DropMessage.Description, resultData.DropMessage.Message);
                }
                var messageCookie = $.cookie("DropMessage");
                if (messageCookie !== null) {
                    toastService = new ToastService();
                    toastService.displayDropMessage();
                }
                deferred.reject(resultData);
            })
            .done(function (resultData, textStatus, jqXHR) {
                $.loading(false);
                if (resultData.Result != "PostJsonResult") {
                    alert("I can only handle PostJsonResult objects!");
                    return;
                }

                if (resultData.RedirectUrl || resultData.Refresh) {
                    if (resultData.DropMessage) {
                        $.cookie("DropMessage", JSON.stringify(resultData.DropMessage), { path: '/' });
                    }

                    if (resultData.Refresh)
                        location.reload(true);
                    else
                        location.href = resultData.RedirectUrl;

                    return;
                }

                if (resultData.DropMessage) {
                    var messageTypes = ["success", "error", "warning", "info"];
                    toastr[messageTypes[resultData.DropMessage
                        .DropMessageType]](resultData.DropMessage.Description, resultData.DropMessage.Message);
                }

                deferred.resolve(resultData);
            });

        return deferred.promise();
    };
}(jQuery));