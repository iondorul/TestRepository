;

/*
 * jQuery hashchange event - v1.3 - 7/21/2010
 * http://benalman.com/projects/jquery-hashchange-plugin/
 * 
 * Copyright (c) 2010 "Cowboy" Ben Alman
 * Dual licensed under the MIT and GPL licenses.
 * http://benalman.com/about/license/
 */
(function ($, e, b) { var c = "hashchange", h = document, f, g = $.event.special, i = h.documentMode, d = "on" + c in e && (i === b || i > 7); function a(j) { j = j || location.href; return "#" + j.replace(/^[^#]*#?(.*)$/, "$1") } $.fn[c] = function (j) { return j ? this.bind(c, j) : this.trigger(c) }; $.fn[c].delay = 50; g[c] = $.extend(g[c], { setup: function () { if (d) { return false } $(f.start) }, teardown: function () { if (d) { return false } $(f.stop) } }); f = (function () { var j = {}, p, m = a(), k = function (q) { return q }, l = k, o = k; j.start = function () { p || n() }; j.stop = function () { p && clearTimeout(p); p = b }; function n() { var r = a(), q = o(m); if (r !== m) { l(m = r, q); $(e).trigger(c) } else { if (q !== m) { location.href = location.href.replace(/#.*/, "") + q } } p = setTimeout(n, $.fn[c].delay) } window.attachEvent && !window.addEventListener && !d && (function () { var q, r; j.start = function () { if (!q) { r = $.fn[c].src; r = r && r + a(); q = $('<iframe tabindex="-1" title="empty"/>').hide().one("load", function () { r || l(a()); n() }).attr("src", r || "javascript:0").insertAfter("body")[0].contentWindow; h.onpropertychange = function () { try { if (event.propertyName === "title") { q.document.title = h.title } } catch (s) { } } } }; j.stop = k; o = function () { return a(q.location.href) }; l = function (v, s) { var u = q.document, t = $.fn[c].domain; if (v !== s) { u.title = h.title; u.open(); t && u.write('<script>document.domain="' + t + '"<\/script>'); u.close(); q.location.hash = v } } })(); return j })() })(jQuery, this);

/*
 * jQuery scrollintoview() plugin and :scrollable selector filter
 *
 * Version 1.8 (14 Jul 2011)
 * Requires jQuery 1.4 or newer
 *
 * Copyright (c) 2011 Robert Koritnik
 * Licensed under the terms of the MIT license
 * http://www.opensource.org/licenses/mit-license.php
 */
(function (f) { var c = { vertical: { x: false, y: true }, horizontal: { x: true, y: false }, both: { x: true, y: true }, x: { x: true, y: false }, y: { x: false, y: true } }; var b = { duration: "fast", direction: "both" }; var e = /^(?:html)$/i; var g = function (k, j) { j = j || (document.defaultView && document.defaultView.getComputedStyle ? document.defaultView.getComputedStyle(k, null) : k.currentStyle); var i = document.defaultView && document.defaultView.getComputedStyle ? true : false; var h = { top: (parseFloat(i ? j.borderTopWidth : f.css(k, "borderTopWidth")) || 0), left: (parseFloat(i ? j.borderLeftWidth : f.css(k, "borderLeftWidth")) || 0), bottom: (parseFloat(i ? j.borderBottomWidth : f.css(k, "borderBottomWidth")) || 0), right: (parseFloat(i ? j.borderRightWidth : f.css(k, "borderRightWidth")) || 0) }; return { top: h.top, left: h.left, bottom: h.bottom, right: h.right, vertical: h.top + h.bottom, horizontal: h.left + h.right } }; var d = function (h) { var j = f(window); var i = e.test(h[0].nodeName); return { border: i ? { top: 0, left: 0, bottom: 0, right: 0 } : g(h[0]), scroll: { top: (i ? j : h).scrollTop(), left: (i ? j : h).scrollLeft() }, scrollbar: { right: i ? 0 : h.innerWidth() - h[0].clientWidth, bottom: i ? 0 : h.innerHeight() - h[0].clientHeight }, rect: (function () { var k = h[0].getBoundingClientRect(); return { top: i ? 0 : k.top, left: i ? 0 : k.left, bottom: i ? h[0].clientHeight : k.bottom, right: i ? h[0].clientWidth : k.right } })() } }; f.fn.extend({ scrollintoview: function (j) { j = f.extend({}, b, j); j.direction = c[typeof (j.direction) === "string" && j.direction.toLowerCase()] || c.both; var n = ""; if (j.direction.x === true) { n = "horizontal" } if (j.direction.y === true) { n = n ? "both" : "vertical" } var l = this.eq(0); var i = l.closest(":scrollable(" + n + ")"); if (i.length > 0) { i = i.eq(0); var m = { e: d(l), s: d(i) }; var h = { top: m.e.rect.top - (m.s.rect.top + m.s.border.top), bottom: m.s.rect.bottom - m.s.border.bottom - m.s.scrollbar.bottom - m.e.rect.bottom, left: m.e.rect.left - (m.s.rect.left + m.s.border.left), right: m.s.rect.right - m.s.border.right - m.s.scrollbar.right - m.e.rect.right }; var k = {}; if (j.direction.y === true) { if (h.top < 0) { k.scrollTop = m.s.scroll.top + h.top } else { if (h.top > 0 && h.bottom < 0) { k.scrollTop = m.s.scroll.top + Math.min(h.top, -h.bottom) } } } if (j.direction.x === true) { if (h.left < 0) { k.scrollLeft = m.s.scroll.left + h.left } else { if (h.left > 0 && h.right < 0) { k.scrollLeft = m.s.scroll.left + Math.min(h.left, -h.right) } } } if (!f.isEmptyObject(k)) { if (e.test(i[0].nodeName)) { i = f("html,body") } i.animate(k, j.duration).eq(0).queue(function (o) { f.isFunction(j.complete) && j.complete.call(i[0]); o() }) } else { f.isFunction(j.complete) && j.complete.call(i[0]) } } return this } }); var a = { auto: true, scroll: true, visible: false, hidden: false }; f.extend(f.expr[":"], { scrollable: function (k, i, n, h) { var m = c[typeof (n[3]) === "string" && n[3].toLowerCase()] || c.both; var l = (document.defaultView && document.defaultView.getComputedStyle ? document.defaultView.getComputedStyle(k, null) : k.currentStyle); var o = { x: a[l.overflowX.toLowerCase()] || false, y: a[l.overflowY.toLowerCase()] || false, isRoot: e.test(k.nodeName) }; if (!o.x && !o.y && !o.isRoot) { return false } var j = { height: { scroll: k.scrollHeight, client: k.clientHeight }, width: { scroll: k.scrollWidth, client: k.clientWidth }, scrollableX: function () { return (o.x || o.isRoot) && this.width.scroll > this.width.client }, scrollableY: function () { return (o.y || o.isRoot) && this.height.scroll > this.height.client } }; return m.y && j.scrollableY() || m.x && j.scrollableX() } }) })(jQuery);


function g_clean(obj) {
    for (var property in obj) {
        if (!isNaN(property) || obj.hasOwnProperty(property)) {
            if (typeof obj[property] == "object" || typeof obj[property] == "array")
                g_clean(obj[property]);
            else if (property[0] == '_')
                delete obj[property];
        }
    }
}

function g_localize(o) {
    return o ? o['default'] : "";
}


var app = angular.module('ActionForm', ['ui.sortable', 'ui.spanresize', 'ui.tinymce', 'ngDragDrop']);

// the array is for minification purposes, otherwise dependency injection will not work
app.directive('bsRadiogroup', [ '$compile', '$timeout', '$parse', function ($compile, $timeout, $parse) {
    return {
        restrict: 'A',
        priority: 100,
        transclude: true,
        scope: {
            themodel: '=ngModel',
            // thearray: '@ngOptions',  //  lan 2013.07.31: unused
            defaultval: '@bsRadiogroup'
        },
        template:
            '<div class="bs-radiogroup" style="display:inline;">' +
                '<div class="btn-group" data-toggle="buttons-radio" >' +
                    '<button type="button" class="btn btn-small" ' +
                        'ng-class="{active: item.value == themodel, \'btn-info\': item.value == themodel, \'btn-default\': item.value != themodel}"' +
                        'ng-repeat="item in elements" ' +
                        'ng-click="change(item,this)">{{item.label}} ' +
                    '</button>' +
                '</div>' +
                '<div style="display:none;" class="bs-radiogroup-transclude" ng-transclude></div>' +
            '</div>',
        link: function (scope, element, attrs) {
            scope.display = '--';
            scope.elements = [];
            scope.element = angular.element(".bs-radiogroup").index(element);

            attrs.$observe('bsRadiogroup', function (value) {
                if (value) scope.display = value;
            });

            function guessJsonFromString(collection) {
                //  also see g_localizeMaybeJson comment
                if (collection[0] == '[' || collection[0] == '{') {
                    var parsed = $.parseJSON(collection);
                    if (parsed) {
                        // var newCollection = {};
                        var newCollection = [];
                        $.each(parsed, function (key, value) {
                            // newCollection[key] = g_localize(value);
                            newCollection.push({'label':g_localize(value), 'value':key});
                        });
                        // console.log("New collection: ",newCollection);
                        return newCollection;
                    }
                }
                return collection;
            }

            attrs.$observe('ngOptions', function (value, element) {
                if (angular.isDefined(value)) {
                    var match, loc = {};
                    var NG_OPTIONS_REGEXP = /^\s*(.*?)(?:\s+as\s+(.*?))?(?:\s+group\s+by\s+(.*))?\s+for\s+(?:([\$\w][\$\w\d]*)|(?:\(\s*([\$\w][\$\w\d]*)\s*,\s*([\$\w][\$\w\d]*)\s*\)))\s+in\s+(.*)$/;
                    if (match = value.match(NG_OPTIONS_REGEXP)) {
                        var displayFn = $parse(match[2] || match[1]),
                            valueName = match[4] || match[6],
                            valueFn = $parse(match[2] ? match[1] : valueName),
                            valuesFn = $parse(match[7]);
                        var collection = valuesFn(scope.$parent) || [];
                        collection = guessJsonFromString(collection);   //  lan 2013.08.01
                        angular.forEach(collection, function (value, key) {
                            loc[valueName] = collection[key];
                            scope.elements.push({
                                'label': typeof (loc[valueName]) == 'object' ? loc[valueName].label : displayFn(scope.$parent, loc),
                                'value': typeof (loc[valueName]) == 'object' ? loc[valueName].value : valueFn(scope.$parent, loc),
                            });
                        });
                    }
                }
            });

            scope.$watch('themodel', function () {
                scope.setdefault();
            });

            scope.setdefault = function () {
                angular.forEach(scope.elements, function (value, key) {
                    if (value.value == scope.themodel) scope.display = value.label;
                });
            }

            scope.change = function (itm, dom) {

                if (!itm) {
                    scope.display = scope.defaultval;
                    scope.themodel = "";
                } else {
                    scope.display = itm.label;
                    scope.themodel = itm.value;
                }
            }
            var elements = element.find(".bs-radiogroup-transclude").children();
            if (angular.isObject(elements) && elements.length) {
                angular.forEach(elements, function (value, key) {
                    scope.elements.push(value);
                });
                scope.setdefault();
            }
        },
        replace: true
    };
}]);

// the array is for minification purposes, otherwise dependency injection will not work
app.directive('bsCheckbox', [ '$compile', '$timeout', '$parse', function ($compile, $timeout, $parse) {
    return {
        restrict: 'A',
        scope: {
            themodel: '=ngModel',
            title: '@bsCheckbox',
            initUndefined: '@bsInit'
        },
        template:
            '<button type="button" class="btn btn-small btn-default" data-toggle="button"' +
                'ng-click="change()" ' +
                'ng-class="{\'btn-info\': themodel, \'btn-default\': !themodel}">' +
                '{{title}}' +
            '</button>',
        replace: true,
        link: function (scope, element, attrs) {

            scope.change = function () {
                scope.themodel = !scope.themodel;
            };

            scope.init = false;
            scope.$watch('themodel', function () {
                if (!scope.init) {
                    if (scope.themodel) {
                        element.addClass('active');
                        scope.init = true;
                    } else if (scope.themodel === false || scope.initUndefined) {
                        scope.themodel = false;
                        scope.init = true;
                    }
                }
            });

        }
    };
}]);

// the array is for minification purposes, otherwise dependency injection will not work
app.directive('bsSelectpicker', [ '$compile', '$timeout', '$parse', function ($compile, $timeout, $parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $timeout(function () {
                $(element).selectpicker();
            }, 200);
        }
    };
}]);

app.directive('uiShow', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {

            var expression = attrs.uiShow;

            // optional slide duration.
            var duration = (attrs.uiDuration || "fast");
            var effect = (attrs.uiEffect || "slide");

            // check to see the default display of the element based on the link-time value of the model we are watching.
            if (!scope.$eval(expression)) {
                element.hide();
            }

            // watch the expression in scope context to when it changes - and adjust the visibility of the element accordingly.
            scope.$watch(
                expression,
                function (newValue, oldValue) {

                    // Ignore first-run values since we've already defaulted the element state.
                    if (newValue === oldValue)
                        return;

                    if (newValue) {
                        if (effect == 'fade') {
                            element.stop(true, true).fadeIn(duration);
                        } else {
                            element.stop(true, true).slideDown(duration);
                        }
                    } else {
                        if (effect == 'fade') {
                            element.stop(true, true).fadeOut(duration);
                        } else {
                            element.stop(true, true).slideUp(duration);
                        }
                    }

                }
            );

        }
    };
});

// the array is for minification purposes, otherwise dependency injection will not work
app.directive('uiFocus', [ '$timeout', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            scope.$watch(attrs.uiFocus, function (value) {
                if (attrs.uiFocus) {
                    $timeout(function () {
                        $(element[0]).focus().select();
                    });
                }
            }, true);
        }
    };
}]);

//  Parameters specific begin

// this is to get lists from the server and share them between settings (using the cache offered by the $http service)
// the array is for minification purposes, otherwise dependency injection will not work
app.factory('dataSources', [ '$http', function ($http) {
    return {
        callForData: function (settings, fnReady) {
            var url = g_adminApi + '?method=GetData&alias=' + g_portalAlias + '&mid=' + g_moduleId;
            for (var name in settings)
                if (settings.hasOwnProperty(name)) {
                    url += ('&'+name+'=');
                    url += encodeURIComponent(settings[name]);
                }

            $http({
                method: 'GET',
                url: url,
                cache: true
            }).success(function (data, status) {
                fnReady && fnReady(data);
            });
        }
    };
}]);

// the array is for minification purposes, otherwise dependency injection will not work
app.directive('ctlDatasource', ['$compile', '$timeout', '$parse', 'dataSources', function ($compile, $timeout, $parse, dataSources) {
    return {
        scope: {
            pdef: '=ctlDatasource',
            model: '=updatemodel'
        },
        link: function (scope, element, attrs) {
            $timeout(function () {
                if (scope.pdef.Settings['Items']) {
                    if (!scope.model)
                        scope.model = {};
                    var items = $.parseJSON(scope.pdef.Settings['Items']);
                    scope.items = [];
                    for (i in items)
                        scope.items.push({Value: i, Name: g_localize(items[i])});
                } else {
                    dataSources.callForData(scope.pdef.Settings, function(data) {scope.items = data;});
                }
            });
        }
    };
}]);

function StringToBool(o) {
    //  JSON values aren't necessary strings
    if (typeof(o) !== 'string')
        return o;

    if (o.toLowerCase() == 'true')
        return true;
    else if (o.toLowerCase() == 'false')
        return false;
    else
        return o;
}

function Parameters_AssignDefaultValues(defaultParameters, parameters) {
    for (var i = 0; i < defaultParameters.length; i++) {

        var p = defaultParameters[i];
        if (typeof parameters[p.Id] != 'undefined')
            continue; // this parameter already has a value

        parameters[p.Id] = p.DefaultValue ? g_localize(p.DefaultValue) : '';
        //  maybe we have non-localized default settings
        if (!parameters[p.Id] && !p.DefaultValue && p.Settings && p.Settings['Defaults']) {
            var val = p.Settings['Defaults'];
            //  also see g_localizeMaybeJson; it will throw exception if not json
            if (val && (val[0] == '[' || val[0] == '{')) {
                var processBool = (val[0] != '[');
                val = $.parseJSON(val);
                //  we only can have arrays of bools ?
                if (!processBool) {
                    parameters[p.Id] = {};
                    for (var i=0; val.length > i; ++i)
                        parameters[p.Id][val[i]] = true;
                    return;
                }
            }
            parameters[p.Id] = val;
        }

        // booleans will come as strings, convert them to true bool
        if (typeof(parameters[p.Id]) === 'object') {
            $.each(parameters[p.Id], function(key, value) {
                parameters[p.Id][key] = StringToBool(value);
            });
        } else {
            parameters[p.Id] = StringToBool(parameters[p.Id]);
        }

        // console.log(parameters[p.Id]);
    }
}

//  Parameters specific end

// this is for sharing data
app.factory('sharedData', function () {
    return {
        serverValidators: [],
        resourceVersion: g_resourceVersion,

        notInArray: function (arr, currentItem) {
            return function (input) {
                var i = $.inArray(input.BoundName, arr);
                if (i != currentItem && i != -1)
                    return false;
                return true;
            };
        },

        exceptItem: function (item) {
            return function (input) {
                return item != input.BoundName;
            };
        }
    };
});


function FormCtrl($scope, $http, sharedData, $timeout) {

    $scope.templates = g_templates;
    $scope.jQueryThemes = g_jQueryThemes;
    $scope.sharedData = sharedData;

    function prepareSettings() {
        $scope.watchAll && $scope.watchAll(); // this clears previous watch
        $scope.modified = false;

        $scope.watchAll = $scope.$watch('sharedData.settings', function (newValue, oldValue) {
            if (newValue === oldValue)
                return;
            $scope.modified = true;
        }, true); // this last true does a "deep" watch, otherwise angularJs would compare by reference, which never changes in this case here (arrays)

    }

    $scope.save = function () {

        $timeout(function () {
            $scope.modified = false;
        });

        $http({
            method: 'POST',
            url: g_adminApi + '?method=SaveSettings&alias=' + g_portalAlias + '&mid=' + g_moduleId,
            data: JSON.stringify($scope.sharedData.settings)
        }).success(function (data, status) {

            // update our local copy, since the server may have done some alteration
            $scope.sharedData.settings = data;

            // reset modfied states
            prepareSettings();
        });
    };

    $http({
        method: 'GET',
        url: g_adminApi + '?method=GetSettings&alias=' + g_portalAlias + '&mid=' + g_moduleId
    }).success(function (data, status) {
        $scope.sharedData.settings = data;
        prepareSettings();
    });
}
// for minification purposes
FormCtrl.$inject = ['$scope', '$http', 'sharedData', '$timeout'];


function FieldsCtrl($scope, $http, sharedData, $timeout) {
    $scope.mode = 'edit';
    //$scope.mode = 'layout';
    $scope.fieldDefs = g_fieldDefs;
    $scope.predefFields = g_predefFields;
    $scope.validatorDefs = g_validatorDefs;
    $scope.groupValidatorDefs = g_groupValidatorDefs;
    $scope.sharedData = sharedData;
    $scope.sharedData.fields = [];
    $scope.lang = g_lang;
    $scope.grid = [];

    // this is an array of handler(fields) that will get called on 
    $scope.onSave = [];

    // organiz field defs by group
    $scope.fieldDefGroups = {};
    $.each($scope.fieldDefs, function (k, o) {
        for (var i = 0; i < o.Categories.length; i++) {
            $scope.fieldDefGroups[o.Categories[i]] = $scope.fieldDefGroups[o.Categories[i]] || [];
            $scope.fieldDefGroups[o.Categories[i]].push(o);
        }
    });

    $scope.computeName = function (field) {

        // make source BoundName is initialized - it doesn't come from the server, we're just using it on client sid
        field.BoundName = field.BoundName || field.Name;
        if (!field.AutoName) {
            field.Name = field.BoundName;
            return;
        }

        field.Name = null;
        field.BoundName = field.Title.replace(/[^A-Za-z0-9]/g, "");
    };

    function initParameters(def, field) {
		Parameters_AssignDefaultValues(def.Parameters, field.Parameters);
    }

    $scope.addField = function (def) {

        // make sure gris is built
        if (!$scope.grid)
            $scope.buildGrid();

        var field = {
            _uid: 'newField' + new Date().getTime(),
            _isOpen: true,
            _hasFocus: true,
            AutoName: true,
            "Title": "New Field",
            "Name": "",
            "InputTypeStr": def.Name,
            "InputData": "",
            "IsEnabled": true,
            RowIndex: $scope.grid.length,
            ColSpan: 9,
            ColOffset: 0,
            Parameters: {},
            Actions: []
        };

        initParameters(def, field);

        $scope.sharedData.fields.push(field);
        $scope.computeName(field);

        // refresh grid
        $scope.buildGrid();
    };

    $scope.cloneField = function (field) {
        if (!$scope.grid)
            $scope.buildGrid();

        $scope.sharedData.fields.push($.extend({}, field, {
            _uid: 'newField' + new Date().getTime(),
            _isOpen: true,
            _hasFocus: true,
            RowIndex: $scope.grid.length,
            ColSpan: 9,
            ColOffset: 0
        }));

        // refresh grid
        $scope.buildGrid();
    };

    $scope.prepareData = function (fields) {
        $scope.sharedData.serverValidators = [];
        //$scope.fieldsModified = fields.length > 0;
        $scope.fieldsModified = false;

        $.each(fields, function (i, o) {
            //$scope.fieldsModified = o.FormFieldId > 0 ? false : $scope.modified;
            o._uid = 'field' + o.FormFieldId + new Date().getTime();
            o.AutoName = !o.Name;
            $scope.computeName(o);

            // parse server validators
            if (o.CustomValidator1 && $scope.validatorDefs[o.CustomValidator1] && $scope.validatorDefs[o.CustomValidator1].IsServerSideValidation)
                $scope.sharedData.serverValidators.push({ title: o.Title + ' Field: <em>' + $scope.validatorDefs[o.CustomValidator1].Title + '</em> Failed', value: o.FormFieldId + '-' + $scope.validatorDefs[o.CustomValidator1].Title });
            if (o.CustomValidator2 && $scope.validatorDefs[o.CustomValidator2] && $scope.validatorDefs[o.CustomValidator2].IsServerSideValidation)
                $scope.sharedData.serverValidators.push({ title: o.Title + ' Field: <em>' + $scope.validatorDefs[o.CustomValidator2].Title + '</em> Failed', value: o.FormFieldId + '-' + $scope.validatorDefs[o.CustomValidator2].Title });
            if (o.ValidationGroup && $scope.groupValidatorDefs[o.GroupValidator] && $scope.groupValidatorDefs[o.GroupValidator].IsServerSideValidation)
                $scope.sharedData.serverValidators.push({ title: o.ValidationGroup + ': <em>' + $scope.groupValidatorDefs[o.GroupValidator].Title + '</em> Failed', value: o.ValidationGroup + '-' + $scope.groupValidatorDefs[o.GroupValidator].Title });

            initParameters($scope.fieldDefs[o.InputTypeStr], o);

        });

        $scope.sharedData.serverValidators = $.unique($scope.sharedData.serverValidators);
        $scope.buildGrid();

        // setup a watch to show the save button
        $scope.watchAllFields = $scope.$watch('sharedData.fields', function (newValue, oldValue) {

            if (newValue === oldValue || JSON.stringify(newValue) == JSON.stringify(oldValue))
                return;
            //console.log(JSON.stringify(oldValue));
            //console.log(JSON.stringify(newValue));
            //for (var i in oldValue)
            //    if (oldValue[i] != newValue[i]) {
            //        for (var j in oldValue[i]) {
            //            if (oldValue[i][j] != newValue[i][j]) {
            //                console.log(j);
            //                console.log(oldValue[i][j]);
            //                console.log(newValue[i][j]);
            //            }
            //        }
            //    }
            $scope.fieldsModified = true;
        }, true); // this last true does a "deep" watch, otherwise angularJs would compare by reference, which never changes in this case here (arrays)

        // fieldsModified will be set to true as the child EventsCtrl generats and update when setting up the actions
        $timeout(function () {
           // $scope.fieldsModified = false;
        });
    };

    $scope.actionsLoaded = function () {
        $scope.fieldsModified = false;
    };

    $scope.buildGrid = function () {
        $scope.grid = [];
        $.each($scope.sharedData.fields, function (i, o) {
            if (o.Parameters && o.Parameters['ShowIn'] && !o.Parameters['ShowIn']['Form']) {
                o.ViewOrder = 9999;
                return;
            }

            var toAdd = o.RowIndex - $scope.grid.length;
            for (var irow = 0; irow <= toAdd; irow++)
                $scope.grid.push([]);

            // append this column
            $scope.grid[o.RowIndex].push({
                span: o.ColSpan,
                row: o.RowIndex,
                field: o
            });
        });

        $scope.fixGrid();
    };

    // fills with empty cells to meet the 12 grid system
    $scope.fixGrid = function () {
        for (var irow = 0; irow < $scope.grid.length; irow++)
            $scope.fixGridRow(irow);
        $scope.updateRowIndexes();
    };

    $scope.siblingFields = function (col) {
        return $.grep($scope.grid[col.row], function (c) { return c != col && c.field.Title; });
    };

    $scope.makeFullWidth = function (col) {
        // TODO: take into account other fields on same row
        col.field.ColOffset = 0;
        col.field.ColSpan = 12;
        $scope.fixGridRow(col.row);
    };

    $scope.nextNonEmptyCell = function (list, after) {
        var iCell = $.inArray(after, list);
        if (iCell == -1)
            return null;

        while (++iCell < list.length) {
            if (list[iCell].field.Title)
                return list[iCell];
        }

        return null;
    };

    $scope.fieldDroped = function (event, ui, dragModel, dropModel) {

        var nextFieldAfterClearedOne = $scope.nextNonEmptyCell($scope.grid[dragModel.$modelValue.field.RowIndex], dragModel.$modelValue);
        if (nextFieldAfterClearedOne)
            nextFieldAfterClearedOne.field.ColOffset += dragModel.$modelValue.span;

        // take row index and span of current cell
        dropModel.$modelValue.field = dragModel.$modelValue.field;
        dropModel.$modelValue.field.RowIndex = dropModel.$modelValue.row;
        dropModel.$modelValue.field.ColSpan = dropModel.$modelValue.span;
        dropModel.$modelValue.field.ColOffset = 0;
        dragModel.$modelValue.field = {};

        // also get the offset, it's the span of the previous empty cell(s)
        var row = $scope.grid[dropModel.$modelValue.field.RowIndex];
        var iCell = $.inArray(dropModel.$modelValue, row);
        var iEmptyCell = iCell;
        while (iEmptyCell > 0 && !row[iEmptyCell - 1].field.Title) {
            dropModel.$modelValue.field.ColOffset += row[iEmptyCell - 1].span;
            iEmptyCell--;
        }

        // reset the offset for following field, since we've filled the gap
        if (iCell != row.length - 1 && row[iCell + 1].field.Title)
            row[iCell + 1].field.ColOffset = 0;

        $scope.fixGridRow(dropModel.$modelValue.field.RowIndex);
        if (dropModel.$modelValue.field.RowIndex != dragModel.$modelValue.row)
            $scope.fixGridRow(dragModel.$modelValue.row);
        //$scope.$apply();

        $scope.updateRowIndexes();
    };

    $scope.moveOnNewRow = function (col) {
        // remove from old row and give the space to someone else
        var nextFieldAfterClearedOne = $scope.nextNonEmptyCell($scope.grid[col.field.RowIndex], col);
        if (nextFieldAfterClearedOne)
            nextFieldAfterClearedOne.field.ColOffset += col.span;

        var iCell = $.inArray(col, $scope.grid[col.field.RowIndex]);
        $scope.grid[col.field.RowIndex].splice(iCell, 1);
        $scope.fixGridRow(col.field.RowIndex);

        col.field.ColOffset = 0;
        col.field.ColSpan = 12;
        col.field.RowIndex++;
        $scope.grid.splice(col.field.RowIndex, 0, [col]);
        $scope.fixGridRow(col.field.RowIndex);

        $scope.updateRowIndexes();
    };

    $scope.fieldResized = function (col, diff, position) {

        var row = $scope.grid[col.field.RowIndex];
        var iCell = $.inArray(col, row);

        if (position == 'left') {

            // take space from previous empty space
            if (diff > 0) {
                var effDiff = diff;
                if (col.field.ColOffset + effDiff >= col.field.ColSpan)
                    effDiff = col.field.ColSpan - col.field.ColOffset - 1;

                col.field.ColOffset += effDiff;
                col.field.ColSpan -= effDiff;
            } else {
                var effDiff = diff;
                if (col.field.ColOffset + effDiff < 0)
                    effDiff = -col.field.ColOffset;
                col.field.ColOffset += effDiff;
                col.field.ColSpan -= effDiff;
            }

        } else {

            // increase width
            // but take it out from ColOffset of next field
            var iNextCell = iCell + 1;
            var nextCell = row[iNextCell++];
            while (!nextCell.field.Title && iNextCell < row.length)
                nextCell = row[iNextCell++];
            if (!nextCell.field.Title) {

                // how much width is available for current field?
                var occupiedSpans = 0;
                for (var i = 0; i < iCell; i++) {
                    if (row[i].field.Title) {
                        occupiedSpans += row[i].field.ColOffset + row[i].field.ColSpan;
                    }
                }
                occupiedSpans += row[iCell].field.ColOffset;

                col.field.ColSpan = Math.max(1, Math.min(col.field.ColSpan + diff, 12 - occupiedSpans));
            } else {

                var effDiff = Math.min(diff, nextCell.field.ColOffset);
                if (col.field.ColSpan + effDiff <= 0)
                    effDiff = -col.field.ColSpan - 1;

                col.field.ColSpan += effDiff;
                nextCell.field.ColOffset -= effDiff;
            }
        }
        $scope.fixGridRow(col.field.RowIndex);
    };

    $scope.rowOrderChanged = function (event, ui) {
        $scope.updateRowIndexes();
    };

    $scope.editOrderChanged = function (event, ui) {
        // update row indexes from fields
    };

    $scope.updateRowIndexes = function () {

        // first, remove empty rows
        $scope.grid = $.grep($scope.grid, function (r) { 
            return r.length != 1 || r[0].field.Title;
        });

        $.each($scope.grid, function (irow, row) {
            $.each(row, function (icol, col) {
                col.row = irow;
                col.field.RowIndex = irow;
                col.field.ViewOrder = irow * 12 + $scope.colNumber(col);
            });
        });

        $scope.sharedData.fields.sort(function (a, b) { return a.ViewOrder > b.ViewOrder ? 1 : (a.ViewOrder < b.ViewOrder ? -1 : 0) });
    };

    $scope.colNumber = function (ofCol) {
        var colNum = 0;
        var row = $scope.grid[ofCol.row];
        for (var icol = 0; icol < row.length; icol++) {
            var col = row[icol];
            if (col == ofCol)
                return colNum + col.field.ColOffset;

            if (col.field.Title)
                colNum += col.field.ColOffset;
            colNum += col.span;

        }
        return -1; // ? no found ?
    };

    $scope.fixGridRow = function (irow) {

        var toAdd = irow - $scope.grid.length;
        for (var i = 0; i <= toAdd; i++)
            $scope.grid.push([]);

        // remove all empties
        $scope.grid[irow] = $.grep($scope.grid[irow], function (c) { return c.field.Title; });

        //if (!$scope.grid[irow].length) {
        //    $scope.grid.splice(irow, 1);
        //    return;
        //}

        // now fill new empties
        var totalRow = 0;
        var iAdjust = 0;
        $.each($.extend([], $scope.grid[irow]), function (icol, c) {

            c.span = c.field.ColSpan;
            c.row = c.field.RowIndex;
            c.field.ViewOrder = c.field.RowIndex * 12 + $scope.colNumber(c);

            // apend space before?
            if (c.field.ColOffset) {
                $scope.grid[irow].splice(icol + iAdjust, 0, { row: irow, span: c.field.ColOffset, field: {} });
                iAdjust++;
            }
            totalRow += c.field.ColOffset + c.field.ColSpan;
        });
        $scope.grid[irow].push({ row: irow, span: totalRow > 12 ? 0 : 12 - totalRow, field: {} });
    };

    $scope.list1 = [{ title: 'AngularJS - Drag Me1' }, { title: 'AngularJS - Drag Me2' }];

    $scope.save = function () {

        g_clean($scope.sharedData.fields);
        $timeout(function () {
            $scope.fieldsModified = false;
        });
        // $('#FormFields').scrollintoview();

        $http({
            method: 'POST',
            url: g_adminApi + '?method=SaveFields&alias=' + g_portalAlias + '&mid=' + g_moduleId,
            data: JSON.stringify($scope.sharedData.fields)
        }).success(function (data, status) {

            // update our local copy, since the server may have done some alteration
            $scope.sharedData.fields = data;

            // reset modfied states
            $scope.watchAllFields(); // this clears previous watch
            $scope.prepareData($scope.sharedData.fields);
        });

        // delete entries right away
        $scope.sharedData.fields = $.grep($scope.sharedData.fields, function (e) { return !e.IsDeleted });

    };

    $http({
        method: 'GET',
        url: g_adminApi + '?method=GetFields&alias=' + g_portalAlias + '&mid=' + g_moduleId
    }).success(function (data, status) {
        $scope.sharedData.fields = data;
        $scope.prepareData($scope.sharedData.fields);
    });

}
// for minification purposes
FieldsCtrl.$inject = ['$scope', '$http', 'sharedData', '$timeout'];

function EventCtrl($scope, $http, sharedData, $timeout) {

    $scope.actionDefs = g_actionDefs;
    $scope.actionDefGroups = {};
    $.each(g_actionDefs, function (i, o) {
        if (!$scope.actionDefGroups[o.Settings['Group']])
            $scope.actionDefGroups[o.Settings['Group']] = [];
        $scope.actionDefGroups[o.Settings['Group']].push(o);
    });

    $scope.lang = g_lang;
    $scope.localize = g_localize;
    $scope.actions = [];
    $scope.sharedData = sharedData;

    $scope.init = function (eventName, title, field) {

        $scope.watchAllActions && $scope.watchAllActions(); // this clears previous watch
        $scope.actionsModified = false;
        $scope.eventName = eventName;
        $scope.field = field;
        $scope.eventFriendlyTitle = title;

        if (field) {
            $scope.actions = field.Actions;
            $scope.prepareActions();
        } else {
            loadActions(eventName);
        }

    };

    // options for the rich text editor
    $scope.tinymceOptions = {
        theme: 'advanced',
        plugins: "autolink,lists,style,table,iespell,inlinepopups,preview,searchreplace,print,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,template,advlist,visualblocks",
        theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,sub,sup,|,justifyleft,justifycenter,justifyright,justifyfull,|,bullist,numlist,|,formatselect,fontselect,fontsizeselect",
        theme_advanced_buttons2: "forecolor,backcolor,|,cut,copy,paste,pastetext,pasteword,removeformat,|,search,replace,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,hr",
        theme_advanced_buttons3: "tablecontrols,|,ltr,rtl,|,fullscreen,code,print,preview,visualaid,iespell",
        //theme_advanced_buttons4: "insertlayer,moveforward,movebackward,absolute,|,styleprops,|,cite,abbr,acronym,del,ins,attribs,|,visualchars,nonbreaking,template,pagebreak,restoredraft,visualblocks"

        theme_advanced_statusbar_location: "none",
        theme_advanced_path: false,
        theme_advanced_resizing: false,

        convert_urls: false
    };  


    $scope.addAction = function (actionType) {

        var action = {
            Id: -1,
            FieldId: $scope.field ? $scope.field.FormFieldId : null,
            _uid: 'new' + new Date().getTime(),
            EventName: $scope.eventName,
            //Description: actionType.Title[$scope.lang],
            Parameters: {},
            ActionType: actionType.Id,
            _isOpen: true,
            _isFocus: true
        };

        // copy defaults
        $.each($.grep(actionType.Parameters, function (e) { return e.DefaultValue; }), function (intIndex, objValue) {
            var val = g_localizeMaybeJson(objValue.DefaultValue);
            action.Parameters[objValue.Id] = val;
        });

        $scope.actions.push(action);
    };

    $scope.deleteAction = function (action) {
        action.IsDeleted = true
    };

    $scope.prepareActions = function () {
        // reset modfied states
        $scope.watchAllActions && $scope.watchAllActions(); // this clears previous watch
        $scope.actionsModified = false;

        $.each($scope.actions, function (iAction, action) {
            action._uid = action.Id;
            var actionType = $scope.actionDefs[action.ActionType];

            // copy defaults
            $.each($.grep(actionType.Parameters, function (e) { return e.DefaultValue; }), function (i, p) {
                if (action.Parameters[p.Id])
                    return;

                var val = g_localizeMaybeJson(p.DefaultValue);
                action.Parameters[p.Id] = val;
            });
        });

        // setup a watch to show the save button
        $scope.watchAllActions = $scope.$watch('actions', function (newValue, oldValue) {
            if (newValue === oldValue)
                return;
            $scope.actionsModified = true;
        }, true); // this last true does a "deep" watch, otherwise angularJs would compare by reference, which never changes in this case here (arrays)

        // also resent fields ctrl, if it exist as a parent scope (i.e. we're doing the actions under a button)
        if (typeof $scope.actionsLoaded != "undefined") {
            $scope.actionsLoaded();
            $timeout(function () {
                $scope.actionsLoaded();
            });
        }
    };

    function loadActions(eventName) {
        $http({
            method: 'GET',
            url: g_adminApi + '?method=GetEventActions&alias=' + g_portalAlias + '&mid=' + g_moduleId + '&eventName=' + encodeURIComponent(eventName)
        }).success(function (data, status) {
            $scope.actions = data;
          
            $scope.prepareActions();
        });
    }

    $scope.checkFinal = function (action) {
        return $scope.actionDefs[action.ActionType].Final && $scope.actions.indexOf(action) != $scope.actions.length - 1;
    };

    $scope.getId = function (e) {
        if (!e._uid)
            e._uid = 'e' + new Date().getTime();
        return e._uid;
    };

    $scope.save = function () {

        $timeout(function () {
            $scope.actionsModified = false;
        });

        $http({
            method: 'POST',
            url: g_adminApi + '?method=SaveEventActions&alias=' + g_portalAlias + '&mid=' + g_moduleId + '&eventName=' + encodeURIComponent($scope.eventName),
            data: JSON.stringify($scope.actions)
        }).success(function (data, status) {
            // update our local copy, since the server may have done some alteration

            //$.extend($scope.actions, data);
            $scope.actions = data;
            $.each($scope.actions, function (intIndex, objValue) {
                objValue._uid = objValue.Id;
            });

            // setup a watch to show the save button
            $scope.prepareActions();
        });

        // delete entries right away
        //$.each($.grep($scope.actions, function (e) { return e.IsDeleted }), function (i, o) { $scope.actions.splice($.inArray(o, $scope.actions), 1); });
        //$scope.actions = $.map($scope.actions, function (a) { return a.IsDeleted ? null : a; });
        //$.map($scope.actions, function (a) { return a.IsDeleted ? null : a; });
        $scope.actions = $.grep($scope.actions, function (e) { return !e.IsDeleted });

    };

    //getPortalSettings(-1);
}
// for minification purposes
EventCtrl.$inject = ['$scope', '$http', 'sharedData', '$timeout'];

// TODO: have tooltips load dynamically, maybe from the server
var helpMessages = {
    'Sample':
        'test'
};


// Initialize UI
$(function () {

    //$('.help').each(function () {
    //    var helpId = $(this).attr("data-help");
    //    var msg = helpMessages[helpId] ? helpMessages[helpId] : helpId;
    //    console.log(helpId);
    //    if (msg) {
    //        $(this).tooltip({
    //            html: true,
    //            placement: 'bottom',
    //            title: '<p style="text-align: left;">' + msg + '</p>'
    //        });
    //    }
    //});


    // initialize tooltips
    $(document).on('mouseover', '.help', function () {

        if ($(this).attr('data-original-title'))
            return;

        var helpId = $(this).attr("data-help");
        helpMessages[helpId] && $(this).attr('data-content', helpMessages[helpId]);

        $(this).popover({
            html: true,
            placement: 'top'
        });
    });

    // fix affix - when clicking buttons in the menu the active item sometimes ends up being different than what was clicked
    $(window).hashchange(function () {
        var hash = location.hash;
        $('#navbar').find('li').removeClass('active');
        $('#navbar').find('[href=' + hash + ']').parent().addClass('active');
    });

    // tranform btn-link bootstrap buttons to some oher btn- when hovered
    $(document).on("mouseenter", ".btn-link-animate-trigger", function () {
        $(this).find(".btn-link-animate").each(function () {
            $(this).removeClass("btn-link").addClass("btn-" + $(this).attr("data-link-animate"))
                .stop(true, false).animate({ opacity: 1 })
                .find("i").addClass("icon-white");
        });

    }).on("mouseleave", ".btn-link-animate-trigger", function () {
        $(this).find(".btn-link-animate").each(function () {
            $(this).addClass("btn-link").removeClass("btn-" + $(this).attr("data-link-animate"))
                .stop(true, false).animate({ opacity: 0.7 })
                .find("i").removeClass("icon-white");
        });
    });
    
    $('a[href*="#"]').click(function () {
        var l = $(this).attr('href');
        if (l == '#')
            return;

        // extract hash
        var hash = l.substr(l.indexOf('#'));
        var page = l.substr(0, l.indexOf('#'));
        if (page.toLowerCase() && page.toLowerCase() != window.location.pathname.toLowerCase())
            return;

        $('html, body').animate({
            scrollTop: $(hash).offset().top - 20
        }, 500);

        // also post message to top window to do the scroll
        window.top.postMessage(JSON.stringify({
            type: "af-scroll",
            offset: $(hash).offset().top
        }), "*");

        return false;
    });

    // live status

    $.ajax({
        dataType: "json",
        url: 'http://support.dnnsharp.com/api/action-form?hasStatus=Working&hasStatus=Delivered&pageSize=5'
    }).done(function (features) {
        features.length && $('#pnlLiveStatusActive').show();
        $.each(features, function (i, o) {
            $('#pnlLiveStatusActive .list').append('<li><a href="http://www.dnnsharp.com/Support#opturl=' + encodeURIComponent(o.url) + '" target="_top">' + o.name + '</a> ('+ o.status +')</li>');
        });
    });

    $.ajax({
        dataType: "json",
        url: 'http://support.dnnsharp.com/api/action-form?hasStatus=Feature%20Request&pageSize=10&sortby=score'
    }).done(function (features) {
        features.length && $('#pnlLiveStatus').show();
        $.each(features, function (i, o) {
            $('#pnlLiveStatus .list').append('<li><a href="http://www.dnnsharp.com/Support#opturl=' + encodeURIComponent(o.url) + '" target="_top">' + o.name + ' (<u>vote</u> or <u>fund</u>)</a></li>');
        });
    });

    

    // selects with Other option
    //$(document).on('change', '.select-other', function () {
    //    $(this).parent().find('.other').toggle(!$(this).val());
    //});
    //$('.select-other').change();

    // nice selects
    //$('.selectpicker').selectpicker();
    
    // this autosizes admin iframe so it doesn't have a scrollbar
    if (window.postMessage && window.top) {
        var __prevHeight = 0;
        setInterval(function () {
            var bodyHeight = $('body').height() + 50;
            if (bodyHeight != __prevHeight) {
                __prevHeight = bodyHeight;
                window.top.postMessage(JSON.stringify({
                    type: "af-height",
                    height: __prevHeight
                }), "*");
            }
        }, 200);
    }

    if (window.top != window)
        $('.visible-in-iframe').show();
    else
        $('.visible-in-iframe').hide();

});

