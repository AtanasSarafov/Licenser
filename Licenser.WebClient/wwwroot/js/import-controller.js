let ImportController = (function (commonController) {
    'use strict';

    let cacheKey = "";
    let fileExtension = "";

    function extractSheetHeaders(file, url) {
        let dfd = jQuery.Deferred();

        let formData = new FormData();
        formData.append('file', file);

        $.ajax({
            type: "POST",
            url: url,
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.length === 0) {
                    return commonController.handleError;
                }
                cacheKey = response.cacheKey;
                fileExtension = response.fileExtension;
                dfd.resolve(response.sheetHeader);
            },
            error: commonController.handleError
        });

        return dfd.promise();
    }

    function extractSheetData(columnsMap, url) {
        let dfd = jQuery.Deferred();

        $.post(url, { cacheKey, fileExtension, columnsMap })
            .done(function (response) {
                dfd.resolve(response);
            })
            .fail(commonController.handleError);

        return dfd.promise();
    }

    function importLicense(file, url) {
        let dfd = jQuery.Deferred();

        let formData = new FormData();
        formData.append('file', file);

        $.ajax({
            type: "POST",
            url: url,
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                dfd.resolve(response);
            },
            error: commonController.handleError
        });

        return dfd.promise();
    }

    return {
        extractSheetHeaders: extractSheetHeaders,
        extractSheetData: extractSheetData,
        importLicense: importLicense
    };
})(CommonController);
