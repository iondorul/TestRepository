
angular.module('ui.spanresize', []).value('uiSpanResizeConfig', {}).directive('uiSpanResize', [
  'uiSpanResizeConfig', '$timeout', function (uiSpanResizeConfig, $timeout) {
      return {
          require: '?ngModel',
          link: function (scope, element, attrs, ngModel) {

              var allSpans = 'span0 span1 span2 span3 span4 span5 span6 span7 span8 span9 span10 span11 span12';
              var allOffsets = 'offset0 offset1 offset2 offset3 offset4 offset5 offset6 offset7 offset8 offset9 offset10 offset11 offset12';

              //var calcMaxWidth = function (element) {
              //    element = $(element);
              //    var otherCols = element.parent().prevAll('.col').not(element.parent());
              //    console.log(otherCols);
              //    var maxWidth = element.parent().parent().width() - otherCols.size() * parseInt(element.css('paddingLeft')) - otherCols.size() * parseInt(element.css('paddingRight'));
              //    otherCols.each(function (i, o) {
              //        maxWidth -= $(o).width() - parseInt($(o).css('marginLeft'))
              //    });
              //    return maxWidth;
              //};

              var calcMaxWidth = function (element) {
                  element = $(element);
                  
                  //var s = $(element).parent().prevAll('.col').not($(element).parent()).length;
                  //console.log("size:"+s);
                  var s = 1;
                  var maxWidth = element.parent().parent().width() - s * parseInt(element.css('paddingLeft')) - s * parseInt(element.css('paddingRight'));
                  return maxWidth;
              };

              var calcMinLeft = function (element) {
                  element = $(element);
                  var prevField = element.parent().prevAll('.col:not(.empty):first');
                  if (prevField.size()) {
                      return element.position().left - (prevField.offset().left + prevField.width());
                  } else {
                      return element.position().left - element.parent().parent().offset().left;
                  }
              };


              $timeout(function () {

                  var opts = $.extend({ handles: 'e, w', autoHide: true }, scope.$eval(attrs.uiSpanResize) || {});

                  //if (opts.containment == 'parent.parent')
                  //    opts.containment = element.parent().parent();

                  //opts.maxWidth = $(element).parent().parent().width() - parseInt($(element).css('paddingLeft')) - parseInt($(element).css('paddingRight'))
                  // + parseInt($(element).parent().parent().find('.empty').css('marginLeft'));
                  //opts.start = function (event, ui) {
                  //    ui.originalSize.width = $(ui.element).parent().width() - parseInt($(ui.element).parent().parent().css('marginLeft'));
                  //};

                  //$(ui.element).resizable('option', 'maxHeight', maxWidth);
                  //opts.maxWidth = maxWidth;
                  opts.start = function (event, ui) {

                      //var maxWidth = calcMaxWidth(element);
                      //$(ui.element).resizable('option', 'maxHeight', maxWidth);


                      // reset the width for when it goes absolute
                      ui.originalSize.width = $(ui.element).parent().width() - parseInt($(element).css('paddingLeft')) - parseInt(element.css('paddingRight'));
                      //var maxWidth = element.parent().parent().width() - parseInt($(element).css('paddingLeft')) - parseInt(element.css('paddingRight'));
                      
                  };

                  opts.resize = function (event, ui) {
                      //var maxWidth = calcMaxWidth(element);
                      //var minLeft = calcMinLeft(element);
                      ////console.log(ui.position.left);
                      ////console.log(ui.size.width);
                      ////console.log(minLeft);
                      ////console.log(ui.size.width);
                      ////console.log(maxWidth);
                      //var offset = Math.floor(12 * minLeft / maxWidth);
                      //offset = offset < 0 ? 0 : offset;
                      //var span = Math.ceil(12 * ui.size.width / maxWidth);
                      //span = span < 1 ? 1 : span;

                      //console.log(offset + ',' + span)

                      //ngModel.$modelValue.field.ColOffset = offset;
                      //ngModel.$modelValue.field.ColSpan = span;

                      //$(ui.element).parent().removeClass(allSpans).addClass('span' + span)
                      //$(ui.element).parent().removeClass(allOffsets).addClass('offset' + offset)

                      // resize the the empty span, if any

                      //var emptySpan = $(ui.element).parent().next('.empty');
                      //var remainignSpan = offset + span;
                      //if (remainignSpan < 12) {
                      //    emptySpan.removeClass(allSpans).addClass('span' + (12 - remainignSpan)).show();
                      //} else {
                      //    emptySpan.hide();
                      //}

                  };

                  opts.stop = function (event, ui) {
                     
                      var maxWidth = calcMaxWidth(element);

                      // is this left or right resize?
                      var position = ui.position.left != ui.originalPosition.left ? 'left' : 'right';
                      var diffPx = position == 'left' ? ui.position.left - ui.originalPosition.left : ui.size.width - ui.originalSize.width
                      var diffSpan = position == 'left' ? Math.round(12 * diffPx / maxWidth) : Math.round(12 * diffPx / maxWidth);

                      opts.onResize(ngModel.$modelValue, diffSpan, position);

                      
                      //var maxWidth = calcMaxWidth(element);
                      //var minLeft = calcMinLeft(element);
                      //console.log(ui);
                      ////console.log(ui.size.width);
                      ////console.log(minLeft);
                      ////console.log(ui.size.width);
                      ////console.log(maxWidth);
                      //var offset = Math.floor(12 * minLeft / maxWidth);
                      //offset = offset < 0 ? 0 : offset;
                      //var span = Math.ceil(12 * ui.size.width / maxWidth);
                      //span = span < 1 ? 1 : span;

                      //console.log(offset + ',' + span)

                      ////ngModel.$modelValue.field.ColOffset = offset;
                      ////ngModel.$modelValue.field.ColSpan = span;

                      //opts.onResize(ngModel, offset, span);

                      $(ui.element).css({
                          left: 0,
                          top: 0,
                          width: '',
                          position: 'relative'
                      });

                      //$(ui.element).find('.ui-resizable-handle').hide();

                      scope.fixGridRow(ngModel.$modelValue.field.RowIndex, ngModel.$modelValue);
                      scope.$apply();

                      //// recalc max width
                      //maxWidth = element.parent().parent().width() - parseInt($(element).css('paddingLeft')) - parseInt(element.css('paddingRight'))
                      //  - parseInt(element.parent().parent().find('.empty').css('marginLeft'));
                      //$(ui.element).resizable('option', 'maxHeight', maxWidth);


                  };
                  element.resizable(opts);
              });

              //opts.helper = 'resizable-helper';

              //if (opts.gridCols)
              //    opts.grid = [opts.containment.width() / opts.gridCols, 0];
              //opts.gridCols && 
              //console.log(opts.containment.innerWidth() / opts.gridCols);
              //opts.resize = function (event, ui) {
              //    console.log(ui);
              //    var span = Math.ceil(12 * ui.size.width / opts.containment.width());
              //    $(ui.element).parent().removeClass(allSpans).addClass('span' + span);
              //    $(ui.element).css('width', '');
              //};


          }
      };
  }
]);
