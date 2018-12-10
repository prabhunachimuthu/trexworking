(function (window) {

    if (typeof jQuery === "undefined") {
        throw new Error("jQuery library required");
    }

    if (!String.prototype.trim) {
        String.prototype.trim = function () {
            return this.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, '');
        };
    }
    if (!Array.prototype.sortByObjectProperty) {
        Array.prototype.sortByObjectProperty = function (propName, descending) {
            return this.sort(function (a, b) {
                if (typeof b[propName] == 'number' && typeof a[propName] == 'number') {
                    return (descending) ? b[propName] - a[propName] : a[propName] - b[propName];
                } else if (typeof b[propName] == 'string' && typeof a[propName] == 'string') {
                    return (descending) ? b[propName] > a[propName] : a[propName] > b[propName];
                } else {
                    return this;
                }
            });
        };
    }

    function AppError() {
        var temp = Error.apply(this, arguments);
        temp.name = this.name = "Error";
        this.stack = temp.stack;
        this.message = temp.message;
    }
    AppError.prototype = Object.create(Error.prototype);
    AppError.prototype.constructor = AppError;
    var allCutCopyPasteElements = ["loginschoolcode", "loginpassword", "USERID", "SchoolCode", "schoolcode"];

    var Helpers = {
        setFromValidator: false,
        udiseCodeFormat: /^[0-9]{11,}$/,
        studentIdFormat: /^[0-9]{19,}$/,
        oneToHundredFormat: /^(100|[1-9]|[1-9][0-9])$/,
        oneToThousandFormat: /^(1000|[1-9]|[1-9][0-9]|[1-9][0-9][0-9])$/,
        oneToTenThousandFormat: /^(10000|[1-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9])$/,
        oneToHundredWithDecimalFormat: /^(100|[1-9]|[1-9][0-9]|[1-9]\.\d{0,2}|[1-9][0-9]\.\d{0,2})$/,
        pincodeFormat: /^(400000|4[0-9]{0,5})$/,
        emailFormat: /^([\w-\.]+)@@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/,
        init: function () {
            // initialize
            var self = this;
            //self.hideMask();
            //$(window).on('beforeunload', function () {
            //    self.showMask();
            //});

            //$("input[type=text][readonly],input[type=text][data-val=true],input[type=text][required]").bind("cut copy paste", function (e) {
            //    if (allCutCopyPasteElements.indexOf($(this).attr('name')) != -1) {
            //        return true;
            //    }

            //    e.preventDefault();
            //    return false;
            //});

            //$('input[type=text][readonly],input[type=text][data-val=true],input[type=text][required]').on('input', function () {
            //    //var elem = $(this);
            //    //if (["loginschoolcode", "SchoolCode", "schoolcode"].indexOf(elem.attr("name")) !== -1 && !self.udiseCodeFormat.test(elem.val()))
            //    //    elem.val('');
            //    //else if (["studentid", "USERID"].indexOf(elem.attr("name")) !== -1 && !self.studentIdFormat.test(elem.val()))
            //    //    elem.val('');
            //});
        },
        setPad: function (num, len) {
            while (num.toString().length < len) num = '0' + num;
            return num;
        },
        getTodayDate: function () {
            var date = new Date();
            return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
        },
        isNumberKey: function (event) {
            var charCode = (event.which) ? event.which : event.keyCode;
            if (charCode !== 46 && charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return true;
        },
        isRangeValid: function (event, length) {
            var charCode = (event.which) ? event.which : event.keyCode;
            if (charCode !== 46 && charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            if (charCode == 46)
                return true;

            if (length == 3)
                return this.oneToHundredFormat.test(event.target.value + String.fromCharCode(charCode));
            else if (length == 4)
                return this.oneToThousandFormat.test(event.target.value + String.fromCharCode(charCode));
            else if (length == 5)
                return this.oneToTenThousandFormat.test(event.target.value + String.fromCharCode(charCode));
            else
                return this.oneToHundredFormat.test(event.target.value + String.fromCharCode(charCode))
        },
        isRangeValidWithDecimal: function (event) {
            var charCode = (event.which) ? event.which : event.keyCode;
            if (charCode !== 46 && charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            if (charCode == 46)
                return true;

            if (charCode == 46 || (event.target.value && event.target.value.indexOf(".") !== -1))
                return this.oneToHundredWithDecimalFormat.test(event.target.value + String.fromCharCode(charCode));
            else
                return this.oneToHundredFormat.test(event.target.value + String.fromCharCode(charCode));
        },
        isValidEmail: function (email) {
            return this.emailFormat.test(email);
        },
        isValidPincode: function (event) {
            var charCode = (event.which) ? event.which : event.keyCode;
            if (charCode !== 46 && charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return this.pincodeFormat.test(event.target.value + String.fromCharCode(charCode));
        },
        isAlphabetOrSpace: function (event, element) {
            try {
                var charCode = (event.which) ? event.which : event.keyCode;
                if ((charCode > 64 && charCode < 91) || (charCode > 96 && charCode < 123) || charCode == 32)
                    return true;
                else
                    return false;
            }
            catch (err) {
                alert(err.Description);
            }
        },
        isAlphabet: function (event, element) {
            try {
                var charCode = (event.which) ? event.which : event.keyCode;
                if ((charCode > 64 && charCode < 91) || (charCode > 96 && charCode < 123))
                    return true;
                else
                    return false;
            }
            catch (err) {
                alert(err.Description);
            }
        },
        checkForm: function (element) {
            var form;
            if (element && element.form)
                form = element.form;
            else
                form = $('form:visible')[0];

            if (form === null || form === undefined)
                return;

            var valElements = $(form.elements).filter('[data-val=true]:visible,[required]:visible');
            var tempElements = $(valElements).filter('[tabindex]');
            if ($(tempElements).filter('[tabindex]').length > 0) {
                tempElements = tempElements.sort(function (a, b) {
                    a = a.getAttribute('tabindex');
                    b = b.getAttribute('tabindex');

                    a[1] - b[1];
                });
            }
            for (var i = 0; valElements && i < valElements.length; i++) {
                if (valElements[i].disabled)
                    continue;

                if (valElements[i].type == "file" && $(valElements[i]).attr('data-fileuploaded') == "1") {
                    continue;
                }

                $(valElements[i]).closest('.form-group').removeClass('has-error');
                $("span[data-valmsg-for='" + valElements[i].name + "']").removeClass('help-block').html('');
                //$(valElements[i]).valid();
            }

            var validator = $(form).validate();
            validator.checkForm();
            var errorObj = validator.errorList;
            var errorElement;
            for (var i = 0; errorObj && i < errorObj.length; i++) {
                if (errorObj[i].element.disabled)
                    continue;

                if (errorObj[i].element.type == "file" && $(errorObj[i].element).attr('data-fileuploaded') == "1") {
                    continue;
                }

                if (errorObj[i].element) {
                    $(errorObj[i].element).removeAttr('readonly');
                    validator.element(errorObj[i].element);
                    if (errorElement === null || errorElement === undefined) {
                        errorElement = errorObj[i].element;
                    }
                }
            }
            return errorElement;
        },
        validateInput: function (element) {
            if (typeof $.validator === "undefined") {
                throw new Error("jQuery validate library required");
            }

            if (!this.setFromValidator) {
                this.setFormDefaultValidator();
            }

            var form = element.form;
            var elements = $(form.elements).filter('[data-val=true]:visible,[required]:visible');
            if ($(elements).filter('[tabindex]').length > 0) {
                var elms = [];
                $(elements).each(function () {
                    elms.push({
                        element: $(this)[0],
                        tabindex: parseInt($(this).attr('tabindex'))
                    })
                });
                elms.sortByObjectProperty('tabindex');
                elements = elms;
            }

            var validator = $(form).validate();
            var errorElement, elm;
            for (var i = 0; elements && i < elements.length; i++) {
                elm = elements[i].element || elements[i];
                if (elm === element) {
                    break;
                }

                if (elm.readOnly || elm.disabled) {
                    continue;
                }

                if (elm.type == "file" && $(elm).attr('data-fileuploaded') == "1") {
                    console.log(elm.type)
                    continue;
                }

                isValid = $(elm).valid();
                if (isValid) {
                    $(elements[i]).closest('.form-group').removeClass('has-error');
                    $("span[data-valmsg-for='" + elm.name + "']").removeClass('help-block');
                }
                else {
                    validator.element(elm);
                    if (errorElement === null || errorElement === undefined) {
                        errorElement = elm;
                    }
                    break;
                }
            }
            return errorElement;
        },
        ShowMessage: function (message, type, title) {
            if (message === null || message === undefined)
                return;

            if (message.trim().length == 0)
                return;

            if (title === undefined || title === null) {
                title = '';
            }
            message = message.trim();
            if (alertify) {
                if (!alertify.errorAlert) {
                    //define a new errorAlert base on alert
                    alertify.dialog('errorAlert', function factory() {
                        return {
                            build: function () {
                                var errorHeader = '<span class="fa fa-times-circle fa-2x" '
                                + 'style="vertical-align:middle;color:#e10000;">'
                                + '</span> Error';
                                this.setHeader(errorHeader);
                            }
                        };
                    }, true, 'alert');
                }

                alertify.alert(title, message, type);
            }
            else {
                alert(message);
            }
        },
        showConfirm: function (message, title, url) {
            if (message === null || message === undefined)
                return;

            if (message.trim().length == 0)
                return;

            if (title === undefined || title === null) {
                title = '';
            }
            message = message.trim();
            if (alertify) {
                if (url) {
                    console.log(url);
                    alertify.confirm('', message, function (e) { location.href = url; }
                     , function () {
                     });
                }
                else {
                    alertify.confirm('', message, function (e) { }
                         , function () {
                         });
                }
            }
        },
        showMask: function (isDelay) {
            if (isDelay) {
                setTimeout(function () {
                    document.getElementById("divMask").style.display = "block";
                }, isDelay);
            }
            else {
                document.getElementById("divMask").style.display = "block";
            }
        },
        hideMask: function (isDelay) {
            if (isDelay) {
                setTimeout(function () {
                    document.getElementById("divMask").style.display = "none";
                }, isDelay);
            }
            else {
                document.getElementById("divMask").style.display = "none";
            }
        },
        setFormDefaultValidator: function (config) {
            if (typeof $.validator === "undefined") {
                throw new Error("jQuery validate library required");
            }

            var self = this;
            config = config || {
                focusInvalid: false,
                //onsubmit: false,
                errorClass: "field-validation-error",
                errorElement: "span",
                highlight: function (element) {
                    $(element).closest('.form-group').addClass('has-error');
                    $("span[data-valmsg-for='" + element.name + "']").addClass('help-block');
                },
                unhighlight: function (element) {
                    $(element).closest('.form-group').removeClass('has-error');
                    $("span[data-valmsg-for='" + element.name + "']").removeClass('help-block');
                },
                success: function (element) {
                },
                errorPlacement: function (error, element) {

                },
                ignore: '',
                onfocusin: function (element, event) {
                    $(element).closest('.form-group').removeClass('has-error');
                    $("span[data-valmsg-for='" + element.name + "']").removeClass('help-block').html('');
                },
                onkeyup: function (element, event) {
                    if (element.readOnly || element.disabled || element.type === "file") {
                        return false;
                    }

                    var errElm = Helpers.validateInput(element);
                    errElm = errElm || element;
                    if (errElm !== element) {
                        var nodename = element.type;
                        if (nodename === "text" || nodename === "email" || nodename === "password" || nodename === "textarea") {
                            element.value = '';
                        }
                        else if (element.type === "checkbox" || element.type === "radio") {
                            element.checked = false;
                        }
                    } else {
                        $(element).closest('.form-group').removeClass('has-error');
                        $("span[data-valmsg-for='" + element.name + "']").removeClass('help-block');
                    }

                    //validate specific input field
                    //if (element.name && element.name.toLowerCase().indexOf('pincode') !== -1) {
                    //    if (!self.isValidPincode(event))
                    //        element.value = '';
                    //}
                }
                //,submitHandler: function (form) {
                    
                //}
            };

            if (!self.setFromValidator) {
                $.validator.setDefaults(config);
                self.setFromValidator = true;

                $('form:visible select,form:visible checkbox,form:visible radio').on('keyup change', function () {
                    var element = $(this)[0];
                    if (element.readOnly || element.disabled) {
                        return false;
                    }

                    var errElm = Helpers.validateInput(element);
                    errElm = errElm || element;
                    if (errElm !== element) {
                        if (element.nodeName.toLowerCase() === "select") {
                            element.value = '';
                        }
                        else if (element.nodeName.toLowerCase() === "checkbox" || element.nodeName.toLowerCase() === "radio") {
                            element.checked = false;
                        }
                    };
                });
                $('.input-validation-error').parents('.form-group').addClass('has-error');
                $('.field-validation-error').addClass('text-danger');
            }
        }
    };

    Helpers.init();

    if (typeof define === 'function' && define.amd) {
        define(function () { return Helpers; });
    }
    else if (typeof module === 'object' && module.exports) {
        module.exports = Helpers;
    }
    else {
        window.Helpers = Helpers;
    }
}(window));