/**
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */

/**
 * Implementing Drag and Drop functionality in AngularJS is easier than ever.
 * Demo: http://codef0rmer.github.com/angular-dragdrop/
 *
 * @version 1.0.1
 *
 * (c) 2013 Amit Gharat a.k.a codef0rmer <amit.2006.it@gmail.com> - amitgharat.wordpress.com
 */

var jqyoui = angular.module('ngDragDrop', []).service('ngDragDropService', ['$timeout', function ($timeout) {
    this.callEventCallback = function (scope, callbackName, event, ui, dragModel, dropModel) {
        if (!callbackName) {
            return;
        }
        var args = [event, ui, dragModel, dropModel];
        var match = callbackName.match(/^(.+)\((.+)\)$/);
        if (match !== null) {
            callbackName = match[1];
            values = eval('[' + match[0].replace(/^(.+)\(/, '').replace(/\)/, '') + ']');
            args.push.apply(args, values);
        }
        scope[callbackName].apply(scope, args);
    };

    var $this = this;

    this.invokeDrop = function ($draggable, $droppable, event, ui, dropModel) {
        var
          dragSettings = {},
          dropSettings = {},
          jqyoui_pos = null,
          dragItem = {},
          dropItem = {},
          dropModelValue,
          $droppableDraggable = null,
          droppableScope = $droppable.scope(),
        draggableScope = $draggable.scope();


        $droppableDraggable = $droppable.find('[jqyoui-draggable]:last');
        dropSettings = droppableScope.$eval($droppable.attr('jqyoui-droppable')) || [];
        dragSettings = draggableScope.$eval($draggable.attr('jqyoui-draggable')) || [];

        jqyoui_pos = angular.isArray($this.dragModel) ? dragSettings.index : null;
        dragItem = angular.isArray($this.dragModel) ? $this.dragModel[jqyoui_pos] : $this.dragModel;

        if (angular.isArray(dropModelValue) && dropSettings && dropSettings.index !== undefined) {
            dropItem = dropModelValue[dropSettings.index];
        } else if (!angular.isArray(dropModelValue)) {
            dropItem = dropModelValue;
        } else {
            dropItem = {};
        }

        if (dragSettings.animate === true) {
            this.move($draggable, $droppableDraggable.length > 0 ? $droppableDraggable : $droppable, null, 'fast', dropSettings, null);
            this.move($droppableDraggable.length > 0 && !dropSettings.multiple ? $droppableDraggable : [], $draggable.parent('[jqyoui-droppable]'), jqyoui.startXY, 'fast', dropSettings, function () {
                $timeout(function () {
                    // Do not move this into move() to avoid flickering issue
                    $draggable.css({ 'position': 'relative', 'left': '', 'top': '' });
                    $droppableDraggable.css({ 'position': 'relative', 'left': '', 'top': '' });

                   // this.mutateDraggable(draggableScope, dropSettings, dragSettings, $this.dragModel, dropModel, dropItem, $draggable);
                    this.mutateDroppable(droppableScope, dropSettings, dragSettings, dropModel, dragItem, jqyoui_pos);
                    this.callEventCallback(droppableScope, dropSettings.onDrop, event, ui, $this.dragModel, dropModel);
                }.bind(this));
            }.bind(this));
        } else {
            $timeout(function () {
               // this.mutateDraggable(draggableScope, dropSettings, dragSettings, $this.dragModel, dropModel, dropItem, $draggable);
                this.mutateDroppable(droppableScope, dropSettings, dragSettings, dropModel, dragItem, jqyoui_pos);
                this.callEventCallback(droppableScope, dropSettings.onDrop, event, ui, $this.dragModel, dropModel);
            }.bind(this));
        }
    };

    this.move = function ($fromEl, $toEl, toPos, duration, dropSettings, callback) {
        if ($fromEl.length === 0) {
            if (callback) {
                window.setTimeout(function () {
                    callback();
                }, 300);
            }
            return false;
        }

        var zIndex = 9999,
          fromPos = $fromEl.offset(),
          wasVisible = $toEl && $toEl.is(':visible');

        if (toPos === null && $toEl.length > 0) {
            if ($toEl.attr('jqyoui-draggable') !== undefined && $toEl.attr('ng-model') !== undefined && $toEl.is(':visible') && dropSettings && dropSettings.multiple) {
                toPos = $toEl.offset();
                if (dropSettings.stack === false) {
                    toPos.left += $toEl.outerWidth(true);
                } else {
                    toPos.top += $toEl.outerHeight(true);
                }
            } else {
                toPos = $toEl.css({ 'visibility': 'hidden', 'display': 'block' }).offset();
                $toEl.css({ 'visibility': '', 'display': wasVisible ? '' : 'none' });
            }
        }

        $fromEl.css({ 'position': 'absolute', 'z-index': zIndex })
          .css(fromPos)
          .animate(toPos, duration, function () {
              if (callback) callback();
          });
    };

    this.mutateDroppable = function (scope, dropSettings, dragSettings, dropModel, dragItem, jqyoui_pos) {

        if (dropModel.$modelValue == $this.dragModel.$modelValue)
            return;

        if (angular.isArray(dropModel.$modelValue)) {
            if (dropSettings && dropSettings.index >= 0) {
                dropModel.$modelValue[dropSettings.index] = dragItem;
            } else {
                dropModel.$modelValue.push(dragItem);
            }
            if (dragSettings && dragSettings.placeholder === true) {
                dropModel.$modelValue[dropModel.$modelValue.length - 1]['jqyoui_pos'] = jqyoui_pos;
            }
        } else {

            //dropModel.$modelValue = $.extend(dropModel.$modelValue, $this.dragModel.$modelValue);
            if (dragSettings && dragSettings.placeholder === true) {
                dropModel.$modelValue['jqyoui_pos'] = jqyoui_pos;
            }

            //for (var k in $this.dragModel.$modelValue) {
            //    delete $this.dragModel.$modelValue[k];
            //}
            //dropModel.dirty = true;
            //scope.$apply();
        }
    };

    //this.mutateDraggable = function (scope, dropSettings, dragSettings, dragModel, dropModel, dropItem, $draggable) {
    //    var isEmpty = $.isEmptyObject(angular.copy(dropItem));

    //    if (dragSettings && dragSettings.placeholder) {
    //        if (dragSettings.placeholder != 'keep') {
    //            if (angular.isArray(dragModel.$modelValue) && dragSettings.index !== undefined) {
    //                dragModel.$modelValue[dragSettings.index] = dropItem;
    //            } else {
    //                $this.dragModel = dropModel;
    //            }
    //        }
    //    } else {
    //        if (angular.isArray(dragModel.$modelValue)) {
    //            if (isEmpty) {
    //                if (dragSettings && (dragSettings.placeholder !== true && dragSettings.placeholder !== 'keep')) {
    //                    dragModel.$modelValue.splice(dragSettings.index, 1);
    //                }
    //            } else {
    //                dragModel.$modelValue[dragSettings.index] = dropItem;
    //            }
    //        } else {
    //            // Fix: LIST(object) to LIST(array) - model does not get updated using just scope[dragModel] = {...}
    //            // P.S.: Could not figure out why it happened
    //            //$this.dragModel.$modelValue = dropModel.$modelValue;
    //            //if (scope.$parent) {
    //            //    $this.dragModel.$modelValue = dropModel.$modelValue;
    //            //}
    //        }
    //    }

    //    $draggable.css({ 'z-index': '', 'left': '', 'top': '' });
    //};
}]).directive('jqyouiDraggable', ['ngDragDropService', function (ngDragDropService) {
    return {
        require: ['?jqyouiDroppable', '?ngModel'],
        restrict: 'A',
        link: function (scope, element, attrs, ngModel) {

            // 1 is the index of ngModer, 0 probably is jqyouiDroppable ?
            var dragModel = ngModel[1];

            var dragSettings, zIndex;
            var updateDraggable = function (newValue, oldValue) {
                if (newValue) {
                    dragSettings = scope.$eval(element.attr('jqyoui-draggable')) || [];
                    element
                      .draggable({ disabled: false })
                      .draggable(scope.$eval(attrs.jqyouiOptions) || {})
                      .draggable({
                          start: function (event, ui) {
                              zIndex = $(this).css('z-index');
                              $(this).css('z-index', 99999);
                              jqyoui.startXY = $(this).offset();
                              ngDragDropService.dragModel = dragModel;
                              ngDragDropService.callEventCallback(scope, dragSettings.onStart, event, ui);
                          },
                          stop: function (event, ui) {
                              $(this).css('z-index', zIndex);
                              ngDragDropService.callEventCallback(scope, dragSettings.onStop, event, ui);
                          },
                          drag: function (event, ui) {
                              ngDragDropService.callEventCallback(scope, dragSettings.onDrag, event, ui);
                          },
                          helper: function (event) {
                              return $('<div class="alert alert-warning" style="padding: 0 20px; z-index: 99999;">' + $(this).text() + '</div>');
                          },
                          cursorAt: { left: 40, top: 10 }
                      });
                } else {
                    element.draggable({ disabled: true });
                }
            };
            scope.$watch(function () { return scope.$eval(attrs.drag); }, updateDraggable);
            updateDraggable();
        }
    };
}]).directive('jqyouiDroppable', ['ngDragDropService', function (ngDragDropService) {
    return {
        restrict: 'A',
        priority: 1,
        require: '?ngModel',
        link: function (scope, element, attrs, ngModel) {

            var dropModel = ngModel;

            var updateDroppable = function (newValue, oldValue) {
                if (newValue) {
                    element
                      .droppable({ disabled: false })
                      .droppable(scope.$eval(attrs.jqyouiOptions) || {})
                      .droppable({
                          over: function (event, ui) {
                              var dropSettings = scope.$eval(angular.element(this).attr('jqyoui-droppable')) || [];
                              ngDragDropService.callEventCallback(scope, dropSettings.onOver, event, ui);
                          },
                          out: function (event, ui) {
                              var dropSettings = scope.$eval(angular.element(this).attr('jqyoui-droppable')) || [];
                              ngDragDropService.callEventCallback(scope, dropSettings.onOut, event, ui);
                          },
                          drop: function (event, ui) {
                              ngDragDropService.invokeDrop(angular.element(ui.draggable), angular.element(this), event, ui, dropModel);
                          }
                      });
                } else {
                    element.droppable({ disabled: true });
                }
            };

            scope.$watch(function () { return scope.$eval(attrs.drop); }, updateDroppable);
            updateDroppable();
        }
    };
}]);