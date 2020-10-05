/// <reference path="../lib/jquery/dist/jquery.js" />

let LicensesController = (function (options, importController) {
    'use strict';

    function init() {
        attachEventHandlers();
        $("#pageLoader").hide();
    }

    function attachEventHandlers() {
        $('.export-license-btn').on('click', exportLicense);
        $('.import-license-btn').on('click', importLicense);
    }

    function importLicense(e) {
        var message = 'Are you sure that you want to activate this license?';

        swal({
            title: message,
            text: 'This will automatically activate a license file and the calculations will be available for the current group.',
            type: "warning",
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No',
            focusCancel: true
        })
            .then((isConfirmed) => {
                if (isConfirmed.value) {
                    $("#pageLoader").show();

                    $.get(options.urls.exportLicense, { id: $(this).data('id') })
                        .done(function (data) {
                            var file = new File([new Blob([base64ToArrayBuffer(data.data)], { type: data.type })], "filename.txt", { type: "text/plain", lastModified: new Date() });
                            importController.importLicense(file, options.urls.importLicense).then(function (response) {
                                if (response && response.licenseValidationMessages) {
                                    var errMessage = null;

                                    if (response.licenseValidationMessages.length > 0) {
                                        errMessage = '';

                                        if (typeof response.licenseValidationMessages === 'string') {
                                            errMessage = response.licenseValidationMessages;
                                        } else {
                                            var errMessages = response.licenseValidationMessages;
                                            for (var i = 0; i < errMessages.length; i++) {

                                                errMessage += errMessages[i].message + document.createElement("br").outerHTML;
                                            }
                                        }
                                    }

                                    if (response.licenseValidationMessages.Message) {
                                        console.log(response.licenseValidationMessages);
                                        errMessage = "Invalid License File!";
                                    }

                                    if (errMessage) {
                                        Swal.showValidationMessage(
                                            `${errMessage}`
                                        );
                                    }
                                }

                                window.location.reload();
                            });
                        })
                        .fail(CommonController.handleError);
                }
            });
    }

    function exportLicense() {
        $.ajax({
            url: options.urls.exportLicense,
            method: 'POST',
            data: $('#license_form').serialize(),
            complete: function (resp) {
                if (resp.status === 200) {
                    var response = resp.responseJSON;
                    var blob = new Blob([base64ToArrayBuffer(response.data)], { type: response.type });
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.style.display = 'none';
                    a.href = url;
                    a.download = response.fileName;
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a);
                    $("#pageLoader").hide();
                }
            }
        });
    }

    function base64ToArrayBuffer(base64) {
        var binaryString = window.atob(base64);
        var binaryLen = binaryString.length;
        var bytes = new Uint8Array(binaryLen);
        for (var i = 0; i < binaryLen; i++) {
            var ascii = binaryString.charCodeAt(i);
            bytes[i] = ascii;
        }
        return bytes;
    }

    return {
        init: init
    };
})(options, ImportController);