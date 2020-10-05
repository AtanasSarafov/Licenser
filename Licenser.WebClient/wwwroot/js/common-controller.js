let CommonController = (function (generalOptions) {
    const infoIcons = {
        propertiesWithInfoIcons: ['CO2Gas', 'CO2Water', 'H2SGas', 'H2SWater', 'IronSaturation'],
        propertiesInfoIconsText: {
            'CO2Gas': 'Enter the CO2 concentration in gas phase, <strong>or</strong> enter the dissolved CO2 in water below.',
            'CO2Water': 'Enter the CO2 concentration in gas phase above, <strong>or</strong> enter the dissolved CO2 in water here.',
            'H2SGas': 'Enter the H2S concentration in gas phase, <strong>or</strong> enter the dissolved H2S in water below.',
            'H2SWater': 'Enter the H2S concentration in gas phase above, <strong>or</strong> enter the dissolved H2S in water here.',
            'IronSaturation': 'Check the box if the stream is saturated with iron ions.'
        }
    };
    const UOM = {
        deviationProfile: {
            m: 'm',
            km: 'km'
        },
        trueVerticlDepth: {
            m: 'm',
            km: 'km'
        },
        measuredDepth: {
            m: 'm',
            km: 'km'
        },
        corrosionRate: {
            mmy: 'mm/y',
            mpy: 'mpy'
        },
        corrosionRateTOL: {
            mmy: 'mm/y',
            mpy: 'mpy'
        },
        pittingCorrosionRate: {
            mmy: 'mm/y',
            mpy: 'mpy'
        },
        liquidVelocity: {
            ms: 'm/s',
            fts: 'ft/s'
        },
        gasVelocity: {
            ms: 'm/s',
            fts: 'ft/s'
        },
        apiVelocityLimit: {
            ms: 'm/s',
            fts: 'ft/s'
        },
        pressure: {
            bar: 'bar(a)',
            psi: 'psi(a)'
        },
        temperature: {
            celsius: '&#8451;',
            fahrenheit: '&#8457;'
        },
        distance: {
            m: 'm',
            km: 'km'
        },
        elevation: {
            m: 'm',
            ft: 'ft'
        }
    };

    const flowRegime = {
        types: {
            notSpecified: 0,
            stratified: 1,
            annular: 2,
            bubbleAndSlug: 3,
            froth: 4,
            liquidFull: 5,
            gasFull: 6
        },
        colors: {
            notSpecified: "#0AD5ED",
            stratified: "#1778AE",
            annular: "#00ffa9",
            bubbleAndSlug: "#EE0000",
            froth: "#12AE0E",
            liquidFull: "#F7992A",
            gasFull: "#D530E4"
        },
        labels: {
            notSpecified: "Not Specified",
            stratified: "Stratified",
            annular: "Annular",
            bubbleAndSlug: "Bubble and Slug",
            froth: "Froth",
            liquidFull: "Liquid Full",
            gasFull: "Gas Full"
        }
    };

    const sourServiceRegion = {        
        regions: {
            zero: 0,
            one: 1,
            two: 2,
            three: 3
        },
        colors: {
            zero: "#0AD5ED",
            one: "#1778AE",
            two: "#FFBC42",
            three: "#EE0000"
        },
        labels: {
            zero: "Region 0",
            one: "Region 1",
            two: "Region 2",
            three: "Region 3"
        }
    };

    const distanceFormat = "{0:n1}";

    const maxColumnsCount = 50;
    const minColumnsCount = 1;
    const avoidToSuperscriptLabels = ['N2', 'C2', 'C3'];

    function generateRowHeader(property) {
        let headerText = toSuperscriptLabel(property.label);
        if (property.name === 'CO2Water' || property.name === 'H2SWater') {
            headerText = `<i>${headerText}</i>`;
        }

        if (infoIcons.propertiesWithInfoIcons.includes(property.name)) {
            headerText = `${headerText} <span class="icons-sprite icon-info float-right custom-info-icon" data-html="true" data-toggle="tooltip" data-placement="top" title="${infoIcons.propertiesInfoIconsText[property.name]}"></span>`;
        }

        return headerText;
    }

    function handleHotBeforeOnCellMouseDown(event, coords, element) {
        event.stopImmediatePropagation();
    }

    function stripHtml(string) {
        if (!string) {
            return '';
        }

        return string.replace(/<\/?[^>]+(>|$)/g, "");
    }

    function hasHtml(string) {
        if (!string) {
            return false;
        }

        return /<\/?[^>]+(>|$)/.test(string);
    }

    function handleError() {
        $("#pageLoader").hide();

        swal('Error', `Error occurred. Please, contact ${generalOptions.supportEmail}`, 'error');
    }

    function handleInvalidLicenseError() {
        $("#pageLoader").hide();
        swal('Invalid License!', `Your license is invalid. The calculations will not be able to be run. Please import valid license file or contact ${generalOptions.supportEmail}`, 'error');
    }

    function getErrorsSummary(errors, title = 'Stream number: ') {
        let errorMsgs = [];
        (errors || []).forEach(function (errorMsg) {
            errorMsgs.push(title + errorMsg.key);
            errorMsg.value.forEach(function (err) {
                let currentError = '';
                if (err.isMissing) {
                    currentError += err.parameterName + ' is required';
                } else if (err.isOutOfRange) {
                    currentError += err.parameterName + ' is out of range';
                }

                errorMsgs.push(currentError);
            });

            errorMsgs.push('');
        });

        return errorMsgs;
    }

    function attachHorizontalScrollEvent() {
        $('.ht_master .wtHolder, .results-grid .inner').on('scroll', function (e) {
            $('.ht_master .wtHolder, .results-grid .inner').scrollLeft(e.target.scrollLeft);
        });
    }

    function getDropdownMenuConfig(tableSelector) {
        return {
            items: {
                'col_left': {
                    disabled: function () {
                        return !canInsertColumn(tableSelector);
                    }
                },
                'col_right': {
                    disabled: function () {
                        return !canInsertColumn(tableSelector);
                    }
                },
                'remove_col': {
                    disabled: function () {
                        return !canRemoveColumn(tableSelector);
                    }
                }
            }
        };
    }

    function getHtColCount(tableSelector) {
        return $(tableSelector).handsontable('countCols');
    }

    // Returns indexes of the currently selected cells as an array of arrays [[startRow, startCol, endRow, endCol],...]
    function getHtSelectedColIndex(tableSelector) {
        return $(tableSelector).handsontable('getSelected')[0][1];
    }

    function canInsertColumn(tableSelector) {
        return $(tableSelector).handsontable('countCols') < maxColumnsCount;
    }

    function canRemoveColumn(tableSelector) {
        return $(tableSelector).handsontable('countCols') > minColumnsCount;
    }

    function toggleExtendedWaterVisibility(show, plugin, tableInstance, rowConfig, renderCalledManually) {
        if (show) {
            hideMinimumWaterAnalysisRows(plugin, tableInstance, rowConfig, renderCalledManually);
        } else {
            hideExtendedWaterAnalysisRows(plugin, tableInstance, rowConfig, renderCalledManually);
        }
    }

    function toggleExtendedGasVisibility(show, plugin, tableInstance, rowConfig, renderCalledManually) {
        if (show) {
            plugin.showRows(rowConfig.extendedGasIndexes);
        } else {
            plugin.hideRows(rowConfig.extendedGasIndexes);
        }

        renderCalledManually = true;
        tableInstance.render();
        renderCalledManually = false;
    }

    function hideMinimumWaterAnalysisRows(plugin, tableInstance, rowConfig, renderCalledManually) {
        plugin.hideRows(rowConfig.minimumWaterIndexes);
        plugin.showRows(rowConfig.extendedWaterIndexes);

        renderCalledManually = true;
        tableInstance.render();
        renderCalledManually = false;
    }

    function hideExtendedWaterAnalysisRows(plugin, tableInstance, rowConfig, renderCalledManually) {
        plugin.hideRows(rowConfig.extendedWaterIndexes);
        plugin.showRows(rowConfig.minimumWaterIndexes);

        renderCalledManually = true;
        tableInstance.render();
        renderCalledManually = false;
    }

    function toSuperscriptLabel(label) {
        if (avoidToSuperscriptLabels.includes(label)) {
            return label;
        }
        return label.replace("2", "₂").replace("3", "³");
    }

    function handleGridResize(inputGridSelector) {
        $(inputGridSelector + " .ht_master.handsontable thead th").each(function () {
            let currentCellWidth = $(this).width();
            let currentCellIndex = $(this).index();

            $("#stream-results th.result").eq(currentCellIndex).outerWidth(currentCellWidth);
            $("#materialSelection-results th.result").eq(currentCellIndex).outerWidth(currentCellWidth);
        });
    }

    function attachCalculationSettingsEventHandlers(inputsHotInstance, inputsRowsPlugin, unitsHotInstance, unitsRowsPlugin, rowConfig, renderCalledManually) {
        $('#extended-water').on('click', function () {
            hideMinimumWaterAnalysisRows(inputsRowsPlugin, inputsHotInstance, rowConfig, renderCalledManually);
            hideMinimumWaterAnalysisRows(unitsRowsPlugin, unitsHotInstance, rowConfig, renderCalledManually);
            handsontableRefresh(inputsHotInstance);
        });

        $('#minimum-water').on('click', function () {
            hideExtendedWaterAnalysisRows(inputsRowsPlugin, inputsHotInstance, rowConfig, renderCalledManually);
            hideExtendedWaterAnalysisRows(unitsRowsPlugin, unitsHotInstance, rowConfig, renderCalledManually);
        });

        $('#extended-gas').on('click', function () {
            toggleExtendedGasVisibility(true, inputsRowsPlugin, inputsHotInstance, rowConfig, renderCalledManually);
            toggleExtendedGasVisibility(true, unitsRowsPlugin, unitsHotInstance, rowConfig, renderCalledManually);
            handsontableRefresh(inputsHotInstance);
        });

        $('#co2h2sonly').on('click', function () {
            toggleExtendedGasVisibility(false, inputsRowsPlugin, inputsHotInstance, rowConfig, renderCalledManually);
            toggleExtendedGasVisibility(false, unitsRowsPlugin, unitsHotInstance, rowConfig, renderCalledManually);
        });
    }

    function attachSelectElementInTableChangeEvent() {
        $('body').on('change', 'table select', function () {
            $(this).closest('td').next().text($(this).val());
        });
    }

    function handsontableRefresh(inputsHotInstance) {
        setTimeout(function () {
            inputsHotInstance.render();
        });
    }

    function showMissingInputsErrMsg() {
        swal('Error', 'Calculation was not successful – please check the inputs!', 'error');
    }

    return {
        disableCellSelection: handleHotBeforeOnCellMouseDown,
        stripHtml: stripHtml,
        hasHtml: hasHtml,
        handleError: handleError,
        handleInvalidLicenseError: handleInvalidLicenseError,
        infoIcons: infoIcons,
        UOM: UOM,
        flowRegime: flowRegime,
        sourServiceRegion: sourServiceRegion,
        distanceFormat: distanceFormat,
        getErrorsSummary: getErrorsSummary,
        generateRowHeader: generateRowHeader,
        attachHorizontalScrollEvent: attachHorizontalScrollEvent,
        getDropdownMenuConfig: getDropdownMenuConfig,
        handleGridResize: handleGridResize,
        toggleExtendedWaterVisibility: toggleExtendedWaterVisibility,
        toggleExtendedGasVisibility: toggleExtendedGasVisibility,
        hideMinimumWaterAnalysisRows: hideMinimumWaterAnalysisRows,
        hideExtendedWaterAnalysisRows: hideExtendedWaterAnalysisRows,
        attachCalculationSettingsEventHandlers: attachCalculationSettingsEventHandlers,
        attachSelectElementInTableChangeEvent: attachSelectElementInTableChangeEvent,
        getHtColCount: getHtColCount,
        getHtSelectedColIndex: getHtSelectedColIndex,
        handsontableRefresh: handsontableRefresh,
        showMissingInputsErrMsg: showMissingInputsErrMsg
    };
})(generalOptions);