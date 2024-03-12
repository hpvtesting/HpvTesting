(function ($) {
    var $jQval = $.validator;
    var adapters = [];

    /* --- Helpers --- */
    function setValidationValues(options, ruleName, value) {
        options.rules[ruleName] = value;
        if (options.message) {
            options.messages[ruleName] = options.message;
        }
    }

    function splitAndTrim(value) {
        return value.replace(/^\s+|\s+$/g, "").split(/\s*,\s*/g);
    }

    function escapeAttributeValue(value) {
        // As mentioned on http://api.jquery.com/category/selectors/
        return value.replace(/([!"#$%&'()*+,./:;<=>?@\[\\\]^`{|}~])/g, "\\$1");
    }

    function getModelPrefix(fieldName) {
        return fieldName.substr(0, fieldName.lastIndexOf(".") + 1);
    }

    function appendModelPrefix(value, prefix) {
        if (value.indexOf("*.") === 0) {
            value = value.replace("*.", prefix);
        }
        return value;
    }

    /* --- Override default validation methods --- */
    $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3})?(?:[\.,]\d+)?$/.test(value);
    };

    $.validator.methods.range = function (value, element, param) {
        var globalizedValue = value.replace(",", ".");
        return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
    };

    $.validator.methods.date = function (value, element) {
        return this.optional(element) ||
            /^\d\d?\-\d\d?\-\d\d\d\d$/.test(value) ||
            /^\d\d\d\d\-\d\d?\-\d\d?$/.test(value);
    };

    /* --- Validation methods --- */
    $jQval.addMethod("__dummy__",
        function (value, element, params) {
            return true;
        });

    $jQval.addMethod("regex",
        function (value, element, params) {
            var match;
            if (this.optional(element)) {
                return true;
            }

            match = new RegExp(params).exec(value);
            return (match && (match.index === 0) && (match[0].length === value.length));
        });

    $jQval.addMethod("nonalphamin",
        function (value, element, nonalphamin) {
            var match;
            if (nonalphamin) {
                match = value.match(/\W/g);
                match = match && match.length >= nonalphamin;
            }
            return match;
        });

    $jQval.addMethod("greaterThan",
        function (value, element, param) {
            // bind to the blur event of the target in order to revalidate whenever the target field is updated
            // TODO find a way to bind the event just once, avoiding the unbind-rebind overhead
            var target = $(param);
            if (this.settings.onfocusout) {
                target.unbind(".validate-greaterThan")
                    .bind("blur.validate-greaterThan",
                        function () {
                            $(element).valid();
                        });
            }
            return value > target.val();
        });

    /* --- Validation adapter methods --- */
    adapters.add = function (adapterName, params, fn) {
        /// <summary>Adds a new adapter to convert unobtrusive HTML into a jQuery Validate validation.</summary>
        /// <param name="adapterName" type="String">The name of the adapter to be added. This matches the name used
        /// in the data-val-nnnn HTML attribute (where nnnn is the adapter name).</param>
        /// <param name="params" type="Array" optional="true">[Optional] An array of parameter names (strings) that will
        /// be extracted from the data-val-nnnn-mmmm HTML attributes (where nnnn is the adapter name, and
        /// mmmm is the parameter name).</param>
        /// <param name="fn" type="Function">The function to call, which adapts the values from the HTML
        /// attributes into jQuery Validate rules and/or messages.</param>
        /// <returns type="jQuery.validator.unobtrusive.adapters" />
        if (!fn) { // Called with no params, just a function
            fn = params;
            params = [];
        }
        this.push({ name: adapterName, params: params, adapt: fn });
        return this;
    };

    adapters.addBool = function (adapterName, ruleName) {
        /// <summary>Adds a new adapter to convert unobtrusive HTML into a jQuery Validate validation, where
        /// the jQuery Validate validation rule has no parameter values.</summary>
        /// <param name="adapterName" type="String">The name of the adapter to be added. This matches the name used
        /// in the data-val-nnnn HTML attribute (where nnnn is the adapter name).</param>
        /// <param name="ruleName" type="String" optional="true">[Optional] The name of the jQuery Validate rule. If not provided, the value
        /// of adapterName will be used instead.</param>
        /// <returns type="jQuery.validator.unobtrusive.adapters" />
        return this.add(adapterName,
            function (options) {
                setValidationValues(options, ruleName || adapterName, true);
            });
    };

    adapters.addMinMax = function (adapterName, minRuleName, maxRuleName, minMaxRuleName, minAttribute, maxAttribute) {
        /// <summary>Adds a new adapter to convert unobtrusive HTML into a jQuery Validate validation, where
        /// the jQuery Validate validation has three potential rules (one for min-only, one for max-only, and
        /// one for min-and-max). The HTML parameters are expected to be named -min and -max.</summary>
        /// <param name="adapterName" type="String">The name of the adapter to be added. This matches the name used
        /// in the data-val-nnnn HTML attribute (where nnnn is the adapter name).</param>
        /// <param name="minRuleName" type="String">The name of the jQuery Validate rule to be used when you only
        /// have a minimum value.</param>
        /// <param name="maxRuleName" type="String">The name of the jQuery Validate rule to be used when you only
        /// have a maximum value.</param>
        /// <param name="minMaxRuleName" type="String">The name of the jQuery Validate rule to be used when you
        /// have both a minimum and maximum value.</param>
        /// <param name="minAttribute" type="String" optional="true">[Optional] The name of the HTML attribute that
        /// contains the minimum value. The default is "min".</param>
        /// <param name="maxAttribute" type="String" optional="true">[Optional] The name of the HTML attribute that
        /// contains the maximum value. The default is "max".</param>
        /// <returns type="jQuery.validator.unobtrusive.adapters" />
        return this.add(adapterName,
            [minAttribute || "min", maxAttribute || "max"],
            function (options) {
                var min = options.params.min,
                    max = options.params.max;

                if (min && max) {
                    setValidationValues(options, minMaxRuleName, [min, max]);
                } else if (min) {
                    setValidationValues(options, minRuleName, min);
                } else if (max) {
                    setValidationValues(options, maxRuleName, max);
                }
            });
    };

    adapters.addSingleVal = function (adapterName, attribute, ruleName) {
        /// <summary>Adds a new adapter to convert unobtrusive HTML into a jQuery Validate validation, where
        /// the jQuery Validate validation rule has a single value.</summary>
        /// <param name="adapterName" type="String">The name of the adapter to be added. This matches the name used
        /// in the data-val-nnnn HTML attribute(where nnnn is the adapter name).</param>
        /// <param name="attribute" type="String">[Optional] The name of the HTML attribute that contains the value.
        /// The default is "val".</param>
        /// <param name="ruleName" type="String" optional="true">[Optional] The name of the jQuery Validate rule. If not provided, the value
        /// of adapterName will be used instead.</param>
        /// <returns type="jQuery.validator.unobtrusive.adapters" />
        return this.add(adapterName,
            [attribute || "val"],
            function (options) {
                setValidationValues(options, ruleName || adapterName, options.params[attribute]);
            });
    };

    /* --- Validation adapters --- */
    adapters.addSingleVal("accept", "mimtype");
    adapters.addSingleVal("extension", "extension");
    adapters.addSingleVal("regex", "pattern");
    adapters.addBool("creditcard").addBool("date").addBool("digits").addBool("email").addBool("number").addBool("url");
    adapters.addMinMax("length", "minlength", "maxlength", "rangelength").addMinMax("range", "min", "max", "range");
    adapters.addMinMax("minlength", "minlength").addMinMax("maxlength", "minlength", "maxlength");

    adapters.add("equalto",
        ["other"],
        function (options) {
            var prefix = getModelPrefix(options.element.name),
                other = options.params.other,
                fullOtherName = appendModelPrefix(other, prefix),
                element = $(options
                    .form)
                    .find(":input")
                    .filter("[name='" + escapeAttributeValue(fullOtherName) + "']")[0];

            setValidationValues(options, "equalTo", element);
        });

    adapters.add("required",
        function (options) {
            // jQuery Validate equates "required" with "mandatory" for checkbox elements
            if (options.element.tagName
                .toUpperCase() !==
                "INPUT" ||
                options.element.type.toUpperCase() !== "CHECKBOX") {
                setValidationValues(options, "required", true);
            }
        });

    adapters.add("remote",
        ["url", "type", "additionalfields"],
        function (options) {
            var value = {
                url: options.params.url,
                type: options.params.type || "GET",
                data: {}
            },
                prefix = getModelPrefix(options.element.name);

            $.each(splitAndTrim(options.params.additionalfields || options.element.name),
                function (i, fieldName) {
                    var paramName = appendModelPrefix(fieldName, prefix);
                    value.data[paramName] = function () {
                        return $(options.form)
                            .find(":input")
                            .filter("[name='" + escapeAttributeValue(paramName) + "']")
                            .val();
                    };
                });

            setValidationValues(options, "remote", value);
        });

    adapters.add("password",
        ["min", "nonalphamin", "regex"],
        function (options) {
            if (options.params.min) {
                setValidationValues(options, "minlength", options.params.min);
            }
            if (options.params.nonalphamin) {
                setValidationValues(options, "nonalphamin", options.params.nonalphamin);
            }
            if (options.params.regex) {
                setValidationValues(options, "regex", options.params.regex);
            }
        });

    adapters.add("greaterthan",
        ["other"],
        function (options) {
            var prefix = getModelPrefix(options.element.name),
                other = options.params.other,
                fullOtherName = appendModelPrefix(other, prefix),
                element = $(options
                    .form)
                    .find(":input")
                    .filter("[name='" + escapeAttributeValue(fullOtherName) + "']")[0];

            setValidationValues(options, "greaterThan", element);
        });

    adapters.add("requiredgroup",
        ["properties"],
        function (options) {
            var properties = options.params.properties.split(',');

            setValidationValues(options,
                "required",
                function () {
                    for (var i = 0; i < properties.length; i++) {
                        var propertValue = $.trim($(options.form)
                            .find(":input")
                            .filter("[name='" + escapeAttributeValue(properties[i]) + "']")
                            .val());
                        if (propertValue.length > 0)
                            return false;
                    }
                    return true;
                });
        });

    /* --- Validation jQuery plugin --- */
    $.fn.validation = function () {
        return this.each(function () {
            var $form = $(this);
            var options = {
                rules: {},
                messages: {},

                ignore: [], // Don't ignore hidden fields!

                //errorClass: "input-validation-error",
                //errorElement: "span",
                errorPlacement: function (error, element) {
                    $(element).data('errorMessage', $(error).text());
                },
                highlight: function (element) {
                    var $visibleInputElements = $(element);

                    // TODO datepicker
                    // selectize
                    if ($(element).next().hasClass('selectize-control')) {
                        $visibleInputElements = $(element).next().find('.selectize-input input');
                    }

                    $(element)
                        .data('visibleInputElements', $visibleInputElements)
                        .closest(".form-group")
                        .addClass("has-error");

                    $visibleInputElements.on('focus.validation',
                        function () {
                            $('#validation-tooltip').remove();

                            var $after = $(this).hasClass('kt-select2') ? $(this).next('span').next('span') : ($(this).parent().hasClass('input-group') ? $(this).parent() : $(this));
                            $after
                                .after('<div class="tooltip bottom" role="tooltip" id="validation-tooltip" style="opacity: 1; margin-left: 10px;">' +
                                    '<div class="tooltip-arrow"></div>' +
                                    '<div class="tooltip-inner">' +
                                    $(element).data('errorMessage') +
                                    '</div>' +
                                    '</div>');
                        });
                },
                unhighlight: function (element) {
                    $(element).closest(".form-group").removeClass("has-error");

                    $('#validation-tooltip').remove();
                    //if ($(element).next().attr('id') == 'validation-tooltip') {
                    //    $('#validation-tooltip').remove();
                    //}

                    var $visibleInputElements = $(element).data('visibleInputElements');
                    if ($visibleInputElements) {
                        $visibleInputElements.off('focus.validation blur.validation');
                    }
                }
            };

            // All input elements with validation enabled
            $form.find(":input")
                .filter("[data-val=true]")
                .each(function () {
                    var $this = $(this);
                    var elementName = this.name;

                    // Loop over adapters / validation rules
                    for (var i = 0; i < adapters.length; i++) {
                        var adapter = adapters[i];
                        var prefix = "data-val-" + adapter.name;

                        // Skip unspecified validation rules
                        if ($this.attr(prefix) === undefined)
                            continue;

                        var paramValues = {};
                        $.each(adapter.params,
                            function () {
                                paramValues[this] = $this.attr(prefix + "-" + this);
                            });

                        if (!options.rules[elementName]) {
                            options.rules[elementName] = {};
                            options.messages[elementName] = {};
                        }

                        adapter.adapt({
                            element: $this.get(0),
                            form: $form,
                            message: $this.attr(prefix),
                            params: paramValues,
                            rules: options.rules[elementName],
                            messages: options.messages[elementName]
                        });
                    }
                });

            $.extend(options.rules, { "__dummy__": true }); // I don't know why we should need this?
            $form.validate(options);
        });
    };
}(jQuery));

$(document).mouseup(function (e) {
    var container = $("#validation-tooltip").parent('.form-group').find('.kt-select2');
    if (!container.is(e.target) &&
        container.has(e.target).length === 0) {
        if (container.length > 0) {
            $('#validation-tooltip').remove();
        }
    }
});