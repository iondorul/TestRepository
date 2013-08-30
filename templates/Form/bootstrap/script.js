/*!
* Bootstrap.js by @fat & @mdo
* Copyright 2012 Twitter, Inc.
* http://www.apache.org/licenses/LICENSE-2.0.txt
*/
!function ($) { "use strict"; $(function () { $.support.transition = function () { var transitionEnd = function () { var name, el = document.createElement("bootstrap"), transEndEventNames = { WebkitTransition: "webkitTransitionEnd", MozTransition: "transitionend", OTransition: "oTransitionEnd otransitionend", transition: "transitionend" }; for (name in transEndEventNames) if (void 0 !== el.style[name]) return transEndEventNames[name] }(); return transitionEnd && { end: transitionEnd } }() }) }(window.afjQuery), !function ($) { "use strict"; var dismiss = '[data-dismiss="alert"]', Alert = function (el) { $(el).on("click", dismiss, this.close) }; Alert.prototype.close = function (e) { function removeElement() { $parent.trigger("closed").remove() } var $parent, $this = $(this), selector = $this.attr("data-target"); selector || (selector = $this.attr("href"), selector = selector && selector.replace(/.*(?=#[^\s]*$)/, "")), $parent = $(selector), e && e.preventDefault(), $parent.length || ($parent = $this.hasClass("alert") ? $this : $this.parent()), $parent.trigger(e = $.Event("close")), e.isDefaultPrevented() || ($parent.removeClass("in"), $.support.transition && $parent.hasClass("fade") ? $parent.on($.support.transition.end, removeElement) : removeElement()) }; var old = $.fn.alert; $.fn.alert = function (option) { return this.each(function () { var $this = $(this), data = $this.data("alert"); data || $this.data("alert", data = new Alert(this)), "string" == typeof option && data[option].call($this) }) }, $.fn.alert.Constructor = Alert, $.fn.alert.noConflict = function () { return $.fn.alert = old, this }, $(document).on("click.alert.data-api", dismiss, Alert.prototype.close) }(window.afjQuery), !function ($) { "use strict"; var Button = function (element, options) { this.$element = $(element), this.options = $.extend({}, $.fn.button.defaults, options) }; Button.prototype.setState = function (state) { var d = "disabled", $el = this.$element, data = $el.data(), val = $el.is("input") ? "val" : "html"; state += "Text", data.resetText || $el.data("resetText", $el[val]()), $el[val](data[state] || this.options[state]), setTimeout(function () { "loadingText" == state ? $el.addClass(d).attr(d, d) : $el.removeClass(d).removeAttr(d) }, 0) }, Button.prototype.toggle = function () { var $parent = this.$element.closest('[data-toggle="buttons-radio"]'); $parent && $parent.find(".active").removeClass("active"), this.$element.toggleClass("active") }; var old = $.fn.button; $.fn.button = function (option) { return this.each(function () { var $this = $(this), data = $this.data("button"), options = "object" == typeof option && option; data || $this.data("button", data = new Button(this, options)), "toggle" == option ? data.toggle() : option && data.setState(option) }) }, $.fn.button.defaults = { loadingText: "loading..." }, $.fn.button.Constructor = Button, $.fn.button.noConflict = function () { return $.fn.button = old, this }, $(document).on("click.button.data-api", "[data-toggle^=button]", function (e) { var $btn = $(e.target); $btn.hasClass("btn") || ($btn = $btn.closest(".btn")), $btn.button("toggle") }) }(window.afjQuery), !function ($) { "use strict"; var Carousel = function (element, options) { this.$element = $(element), this.options = options, "hover" == this.options.pause && this.$element.on("mouseenter", $.proxy(this.pause, this)).on("mouseleave", $.proxy(this.cycle, this)) }; Carousel.prototype = { cycle: function (e) { return e || (this.paused = !1), this.options.interval && !this.paused && (this.interval = setInterval($.proxy(this.next, this), this.options.interval)), this }, to: function (pos) { var $active = this.$element.find(".item.active"), children = $active.parent().children(), activePos = children.index($active), that = this; if (!(pos > children.length - 1 || 0 > pos)) return this.sliding ? this.$element.one("slid", function () { that.to(pos) }) : activePos == pos ? this.pause().cycle() : this.slide(pos > activePos ? "next" : "prev", $(children[pos])) }, pause: function (e) { return e || (this.paused = !0), this.$element.find(".next, .prev").length && $.support.transition.end && (this.$element.trigger($.support.transition.end), this.cycle()), clearInterval(this.interval), this.interval = null, this }, next: function () { return this.sliding ? void 0 : this.slide("next") }, prev: function () { return this.sliding ? void 0 : this.slide("prev") }, slide: function (type, next) { var e, $active = this.$element.find(".item.active"), $next = next || $active[type](), isCycling = this.interval, direction = "next" == type ? "left" : "right", fallback = "next" == type ? "first" : "last", that = this; if (this.sliding = !0, isCycling && this.pause(), $next = $next.length ? $next : this.$element.find(".item")[fallback](), e = $.Event("slide", { relatedTarget: $next[0] }), !$next.hasClass("active")) { if ($.support.transition && this.$element.hasClass("slide")) { if (this.$element.trigger(e), e.isDefaultPrevented()) return; $next.addClass(type), $next[0].offsetWidth, $active.addClass(direction), $next.addClass(direction), this.$element.one($.support.transition.end, function () { $next.removeClass([type, direction].join(" ")).addClass("active"), $active.removeClass(["active", direction].join(" ")), that.sliding = !1, setTimeout(function () { that.$element.trigger("slid") }, 0) }) } else { if (this.$element.trigger(e), e.isDefaultPrevented()) return; $active.removeClass("active"), $next.addClass("active"), this.sliding = !1, this.$element.trigger("slid") } return isCycling && this.cycle(), this } } }; var old = $.fn.carousel; $.fn.carousel = function (option) { return this.each(function () { var $this = $(this), data = $this.data("carousel"), options = $.extend({}, $.fn.carousel.defaults, "object" == typeof option && option), action = "string" == typeof option ? option : options.slide; data || $this.data("carousel", data = new Carousel(this, options)), "number" == typeof option ? data.to(option) : action ? data[action]() : options.interval && data.cycle() }) }, $.fn.carousel.defaults = { interval: 5e3, pause: "hover" }, $.fn.carousel.Constructor = Carousel, $.fn.carousel.noConflict = function () { return $.fn.carousel = old, this }, $(document).on("click.carousel.data-api", "[data-slide]", function (e) { var href, $this = $(this), $target = $($this.attr("data-target") || (href = $this.attr("href")) && href.replace(/.*(?=#[^\s]+$)/, "")), options = $.extend({}, $target.data(), $this.data()); $target.carousel(options), e.preventDefault() }) }(window.afjQuery), !function ($) { "use strict"; var Collapse = function (element, options) { this.$element = $(element), this.options = $.extend({}, $.fn.collapse.defaults, options), this.options.parent && (this.$parent = $(this.options.parent)), this.options.toggle && this.toggle() }; Collapse.prototype = { constructor: Collapse, dimension: function () { var hasWidth = this.$element.hasClass("width"); return hasWidth ? "width" : "height" }, show: function () { var dimension, scroll, actives, hasData; if (!this.transitioning) { if (dimension = this.dimension(), scroll = $.camelCase(["scroll", dimension].join("-")), actives = this.$parent && this.$parent.find("> .accordion-group > .in"), actives && actives.length) { if (hasData = actives.data("collapse"), hasData && hasData.transitioning) return; actives.collapse("hide"), hasData || actives.data("collapse", null) } this.$element[dimension](0), this.transition("addClass", $.Event("show"), "shown"), $.support.transition && this.$element[dimension](this.$element[0][scroll]) } }, hide: function () { var dimension; this.transitioning || (dimension = this.dimension(), this.reset(this.$element[dimension]()), this.transition("removeClass", $.Event("hide"), "hidden"), this.$element[dimension](0)) }, reset: function (size) { var dimension = this.dimension(); return this.$element.removeClass("collapse")[dimension](size || "auto")[0].offsetWidth, this.$element[null !== size ? "addClass" : "removeClass"]("collapse"), this }, transition: function (method, startEvent, completeEvent) { var that = this, complete = function () { "show" == startEvent.type && that.reset(), that.transitioning = 0, that.$element.trigger(completeEvent) }; this.$element.trigger(startEvent), startEvent.isDefaultPrevented() || (this.transitioning = 1, this.$element[method]("in"), $.support.transition && this.$element.hasClass("collapse") ? this.$element.one($.support.transition.end, complete) : complete()) }, toggle: function () { this[this.$element.hasClass("in") ? "hide" : "show"]() } }; var old = $.fn.collapse; $.fn.collapse = function (option) { return this.each(function () { var $this = $(this), data = $this.data("collapse"), options = "object" == typeof option && option; data || $this.data("collapse", data = new Collapse(this, options)), "string" == typeof option && data[option]() }) }, $.fn.collapse.defaults = { toggle: !0 }, $.fn.collapse.Constructor = Collapse, $.fn.collapse.noConflict = function () { return $.fn.collapse = old, this }, $(document).on("click.collapse.data-api", "[data-toggle=collapse]", function (e) { var href, $this = $(this), target = $this.attr("data-target") || e.preventDefault() || (href = $this.attr("href")) && href.replace(/.*(?=#[^\s]+$)/, ""), option = $(target).data("collapse") ? "toggle" : $this.data(); $this[$(target).hasClass("in") ? "addClass" : "removeClass"]("collapsed"), $(target).collapse(option) }) }(window.afjQuery), !function ($) { "use strict"; function clearMenus() { $(toggle).each(function () { getParent($(this)).removeClass("open") }) } function getParent($this) { var $parent, selector = $this.attr("data-target"); return selector || (selector = $this.attr("href"), selector = selector && /#/.test(selector) && selector.replace(/.*(?=#[^\s]*$)/, "")), $parent = $(selector), $parent.length || ($parent = $this.parent()), $parent } var toggle = "[data-toggle=dropdown]", Dropdown = function (element) { var $el = $(element).on("click.dropdown.data-api", this.toggle); $("html").on("click.dropdown.data-api", function () { $el.parent().removeClass("open") }) }; Dropdown.prototype = { constructor: Dropdown, toggle: function () { var $parent, isActive, $this = $(this); if (!$this.is(".disabled, :disabled")) return $parent = getParent($this), isActive = $parent.hasClass("open"), clearMenus(), isActive || $parent.toggleClass("open"), $this.focus(), !1 }, keydown: function (e) { var $this, $items, $parent, isActive, index; if (/(38|40|27)/.test(e.keyCode) && ($this = $(this), e.preventDefault(), e.stopPropagation(), !$this.is(".disabled, :disabled"))) { if ($parent = getParent($this), isActive = $parent.hasClass("open"), !isActive || isActive && 27 == e.keyCode) return $this.click(); $items = $("[role=menu] li:not(.divider):visible a", $parent), $items.length && (index = $items.index($items.filter(":focus")), 38 == e.keyCode && index > 0 && index--, 40 == e.keyCode && $items.length - 1 > index && index++, ~index || (index = 0), $items.eq(index).focus()) } } }; var old = $.fn.dropdown; $.fn.dropdown = function (option) { return this.each(function () { var $this = $(this), data = $this.data("dropdown"); data || $this.data("dropdown", data = new Dropdown(this)), "string" == typeof option && data[option].call($this) }) }, $.fn.dropdown.Constructor = Dropdown, $.fn.dropdown.noConflict = function () { return $.fn.dropdown = old, this }, $(document).on("click.dropdown.data-api touchstart.dropdown.data-api", clearMenus).on("click.dropdown touchstart.dropdown.data-api", ".dropdown form", function (e) { e.stopPropagation() }).on("touchstart.dropdown.data-api", ".dropdown-menu", function (e) { e.stopPropagation() }).on("click.dropdown.data-api touchstart.dropdown.data-api", toggle, Dropdown.prototype.toggle).on("keydown.dropdown.data-api touchstart.dropdown.data-api", toggle + ", [role=menu]", Dropdown.prototype.keydown) }(window.afjQuery), !function ($) { "use strict"; var Modal = function (element, options) { this.options = options, this.$element = $(element).delegate('[data-dismiss="modal"]', "click.dismiss.modal", $.proxy(this.hide, this)), this.options.remote && this.$element.find(".modal-body").load(this.options.remote) }; Modal.prototype = { constructor: Modal, toggle: function () { return this[this.isShown ? "hide" : "show"]() }, show: function () { var that = this, e = $.Event("show"); this.$element.trigger(e), this.isShown || e.isDefaultPrevented() || (this.isShown = !0, this.escape(), this.backdrop(function () { var transition = $.support.transition && that.$element.hasClass("fade"); that.$element.parent().length || that.$element.appendTo(document.body), that.$element.show(), transition && that.$element[0].offsetWidth, that.$element.addClass("in").attr("aria-hidden", !1), that.enforceFocus(), transition ? that.$element.one($.support.transition.end, function () { that.$element.focus().trigger("shown") }) : that.$element.focus().trigger("shown") })) }, hide: function (e) { e && e.preventDefault(), e = $.Event("hide"), this.$element.trigger(e), this.isShown && !e.isDefaultPrevented() && (this.isShown = !1, this.escape(), $(document).off("focusin.modal"), this.$element.removeClass("in").attr("aria-hidden", !0), $.support.transition && this.$element.hasClass("fade") ? this.hideWithTransition() : this.hideModal()) }, enforceFocus: function () { var that = this; $(document).on("focusin.modal", function (e) { that.$element[0] === e.target || that.$element.has(e.target).length || that.$element.focus() }) }, escape: function () { var that = this; this.isShown && this.options.keyboard ? this.$element.on("keyup.dismiss.modal", function (e) { 27 == e.which && that.hide() }) : this.isShown || this.$element.off("keyup.dismiss.modal") }, hideWithTransition: function () { var that = this, timeout = setTimeout(function () { that.$element.off($.support.transition.end), that.hideModal() }, 500); this.$element.one($.support.transition.end, function () { clearTimeout(timeout), that.hideModal() }) }, hideModal: function () { this.$element.hide().trigger("hidden"), this.backdrop() }, removeBackdrop: function () { this.$backdrop.remove(), this.$backdrop = null }, backdrop: function (callback) { var animate = this.$element.hasClass("fade") ? "fade" : ""; if (this.isShown && this.options.backdrop) { var doAnimate = $.support.transition && animate; this.$backdrop = $('<div class="modal-backdrop ' + animate + '" />').appendTo(document.body), this.$backdrop.click("static" == this.options.backdrop ? $.proxy(this.$element[0].focus, this.$element[0]) : $.proxy(this.hide, this)), doAnimate && this.$backdrop[0].offsetWidth, this.$backdrop.addClass("in"), doAnimate ? this.$backdrop.one($.support.transition.end, callback) : callback() } else !this.isShown && this.$backdrop ? (this.$backdrop.removeClass("in"), $.support.transition && this.$element.hasClass("fade") ? this.$backdrop.one($.support.transition.end, $.proxy(this.removeBackdrop, this)) : this.removeBackdrop()) : callback && callback() } }; var old = $.fn.modal; $.fn.modal = function (option) { return this.each(function () { var $this = $(this), data = $this.data("modal"), options = $.extend({}, $.fn.modal.defaults, $this.data(), "object" == typeof option && option); data || $this.data("modal", data = new Modal(this, options)), "string" == typeof option ? data[option]() : options.show && data.show() }) }, $.fn.modal.defaults = { backdrop: !0, keyboard: !0, show: !0 }, $.fn.modal.Constructor = Modal, $.fn.modal.noConflict = function () { return $.fn.modal = old, this }, $(document).on("click.modal.data-api", '[data-toggle="modal"]', function (e) { var $this = $(this), href = $this.attr("href"), $target = $($this.attr("data-target") || href && href.replace(/.*(?=#[^\s]+$)/, "")), option = $target.data("modal") ? "toggle" : $.extend({ remote: !/#/.test(href) && href }, $target.data(), $this.data()); e.preventDefault(), $target.modal(option).one("hide", function () { $this.focus() }) }) }(window.afjQuery), !function ($) { "use strict"; var Tooltip = function (element, options) { this.init("tooltip", element, options) }; Tooltip.prototype = { constructor: Tooltip, init: function (type, element, options) { var eventIn, eventOut; this.type = type, this.$element = $(element), this.options = this.getOptions(options), this.enabled = !0, "click" == this.options.trigger ? this.$element.on("click." + this.type, this.options.selector, $.proxy(this.toggle, this)) : "manual" != this.options.trigger && (eventIn = "hover" == this.options.trigger ? "mouseenter" : "focus", eventOut = "hover" == this.options.trigger ? "mouseleave" : "blur", this.$element.on(eventIn + "." + this.type, this.options.selector, $.proxy(this.enter, this)), this.$element.on(eventOut + "." + this.type, this.options.selector, $.proxy(this.leave, this))), this.options.selector ? this._options = $.extend({}, this.options, { trigger: "manual", selector: "" }) : this.fixTitle() }, getOptions: function (options) { return options = $.extend({}, $.fn[this.type].defaults, options, this.$element.data()), options.delay && "number" == typeof options.delay && (options.delay = { show: options.delay, hide: options.delay }), options }, enter: function (e) { var self = $(e.currentTarget)[this.type](this._options).data(this.type); return self.options.delay && self.options.delay.show ? (clearTimeout(this.timeout), self.hoverState = "in", this.timeout = setTimeout(function () { "in" == self.hoverState && self.show() }, self.options.delay.show), void 0) : self.show() }, leave: function (e) { var self = $(e.currentTarget)[this.type](this._options).data(this.type); return this.timeout && clearTimeout(this.timeout), self.options.delay && self.options.delay.hide ? (self.hoverState = "out", this.timeout = setTimeout(function () { "out" == self.hoverState && self.hide() }, self.options.delay.hide), void 0) : self.hide() }, show: function () { var $tip, inside, pos, actualWidth, actualHeight, placement, tp; if (this.hasContent() && this.enabled) { switch ($tip = this.tip(), this.setContent(), this.options.animation && $tip.addClass("fade"), placement = "function" == typeof this.options.placement ? this.options.placement.call(this, $tip[0], this.$element[0]) : this.options.placement, inside = /in/.test(placement), $tip.detach().css({ top: 0, left: 0, display: "block" }).insertAfter(this.$element), pos = this.getPosition(inside), actualWidth = $tip[0].offsetWidth, actualHeight = $tip[0].offsetHeight, inside ? placement.split(" ")[1] : placement) { case "bottom": tp = { top: pos.top + pos.height, left: pos.left + pos.width / 2 - actualWidth / 2 }; break; case "top": tp = { top: pos.top - actualHeight, left: pos.left + pos.width / 2 - actualWidth / 2 }; break; case "left": tp = { top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left - actualWidth }; break; case "right": tp = { top: pos.top + pos.height / 2 - actualHeight / 2, left: pos.left + pos.width } } $tip.offset(tp).addClass(placement).addClass("in") } }, setContent: function () { var $tip = this.tip(), title = this.getTitle(); $tip.find(".tooltip-inner")[this.options.html ? "html" : "text"](title), $tip.removeClass("fade in top bottom left right") }, hide: function () { function removeWithAnimation() { var timeout = setTimeout(function () { $tip.off($.support.transition.end).detach() }, 500); $tip.one($.support.transition.end, function () { clearTimeout(timeout), $tip.detach() }) } var $tip = this.tip(); return $tip.removeClass("in"), $.support.transition && this.$tip.hasClass("fade") ? removeWithAnimation() : $tip.detach(), this }, fixTitle: function () { var $e = this.$element; ($e.attr("title") || "string" != typeof $e.attr("data-original-title")) && $e.attr("data-original-title", $e.attr("title") || "").removeAttr("title") }, hasContent: function () { return this.getTitle() }, getPosition: function (inside) { return $.extend({}, inside ? { top: 0, left: 0 } : this.$element.offset(), { width: this.$element[0].offsetWidth, height: this.$element[0].offsetHeight }) }, getTitle: function () { var title, $e = this.$element, o = this.options; return title = $e.attr("data-original-title") || ("function" == typeof o.title ? o.title.call($e[0]) : o.title) }, tip: function () { return this.$tip = this.$tip || $(this.options.template) }, validate: function () { this.$element[0].parentNode || (this.hide(), this.$element = null, this.options = null) }, enable: function () { this.enabled = !0 }, disable: function () { this.enabled = !1 }, toggleEnabled: function () { this.enabled = !this.enabled }, toggle: function (e) { var self = $(e.currentTarget)[this.type](this._options).data(this.type); self[self.tip().hasClass("in") ? "hide" : "show"]() }, destroy: function () { this.hide().$element.off("." + this.type).removeData(this.type) } }; var old = $.fn.tooltip; $.fn.tooltip = function (option) { return this.each(function () { var $this = $(this), data = $this.data("tooltip"), options = "object" == typeof option && option; data || $this.data("tooltip", data = new Tooltip(this, options)), "string" == typeof option && data[option]() }) }, $.fn.tooltip.Constructor = Tooltip, $.fn.tooltip.defaults = { animation: !0, placement: "top", selector: !1, template: '<div class="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>', trigger: "hover", title: "", delay: 0, html: !1 }, $.fn.tooltip.noConflict = function () { return $.fn.tooltip = old, this } }(window.afjQuery), !function ($) { "use strict"; var Popover = function (element, options) { this.init("popover", element, options) }; Popover.prototype = $.extend({}, $.fn.tooltip.Constructor.prototype, { constructor: Popover, setContent: function () { var $tip = this.tip(), title = this.getTitle(), content = this.getContent(); $tip.find(".popover-title")[this.options.html ? "html" : "text"](title), $tip.find(".popover-content")[this.options.html ? "html" : "text"](content), $tip.removeClass("fade top bottom left right in") }, hasContent: function () { return this.getTitle() || this.getContent() }, getContent: function () { var content, $e = this.$element, o = this.options; return content = $e.attr("data-content") || ("function" == typeof o.content ? o.content.call($e[0]) : o.content) }, tip: function () { return this.$tip || (this.$tip = $(this.options.template)), this.$tip }, destroy: function () { this.hide().$element.off("." + this.type).removeData(this.type) } }); var old = $.fn.popover; $.fn.popover = function (option) { return this.each(function () { var $this = $(this), data = $this.data("popover"), options = "object" == typeof option && option; data || $this.data("popover", data = new Popover(this, options)), "string" == typeof option && data[option]() }) }, $.fn.popover.Constructor = Popover, $.fn.popover.defaults = $.extend({}, $.fn.tooltip.defaults, { placement: "right", trigger: "click", content: "", template: '<div class="popover"><div class="arrow"></div><div class="popover-inner"><h3 class="popover-title"></h3><div class="popover-content"></div></div></div>' }), $.fn.popover.noConflict = function () { return $.fn.popover = old, this } }(window.afjQuery), !function ($) { "use strict"; function ScrollSpy(element, options) { var href, process = $.proxy(this.process, this), $element = $(element).is("body") ? $(window) : $(element); this.options = $.extend({}, $.fn.scrollspy.defaults, options), this.$scrollElement = $element.on("scroll.scroll-spy.data-api", process), this.selector = (this.options.target || (href = $(element).attr("href")) && href.replace(/.*(?=#[^\s]+$)/, "") || "") + " .nav li > a", this.$body = $("body"), this.refresh(), this.process() } ScrollSpy.prototype = { constructor: ScrollSpy, refresh: function () { var $targets, self = this; this.offsets = $([]), this.targets = $([]), $targets = this.$body.find(this.selector).map(function () { var $el = $(this), href = $el.data("target") || $el.attr("href"), $href = /^#\w/.test(href) && $(href); return $href && $href.length && [[$href.position().top + self.$scrollElement.scrollTop(), href]] || null }).sort(function (a, b) { return a[0] - b[0] }).each(function () { self.offsets.push(this[0]), self.targets.push(this[1]) }) }, process: function () { var i, scrollTop = this.$scrollElement.scrollTop() + this.options.offset, scrollHeight = this.$scrollElement[0].scrollHeight || this.$body[0].scrollHeight, maxScroll = scrollHeight - this.$scrollElement.height(), offsets = this.offsets, targets = this.targets, activeTarget = this.activeTarget; if (scrollTop >= maxScroll) return activeTarget != (i = targets.last()[0]) && this.activate(i); for (i = offsets.length; i--;) activeTarget != targets[i] && scrollTop >= offsets[i] && (!offsets[i + 1] || offsets[i + 1] >= scrollTop) && this.activate(targets[i]) }, activate: function (target) { var active, selector; this.activeTarget = target, $(this.selector).parent(".active").removeClass("active"), selector = this.selector + '[data-target="' + target + '"],' + this.selector + '[href="' + target + '"]', active = $(selector).parent("li").addClass("active"), active.parent(".dropdown-menu").length && (active = active.closest("li.dropdown").addClass("active")), active.trigger("activate") } }; var old = $.fn.scrollspy; $.fn.scrollspy = function (option) { return this.each(function () { var $this = $(this), data = $this.data("scrollspy"), options = "object" == typeof option && option; data || $this.data("scrollspy", data = new ScrollSpy(this, options)), "string" == typeof option && data[option]() }) }, $.fn.scrollspy.Constructor = ScrollSpy, $.fn.scrollspy.defaults = { offset: 10 }, $.fn.scrollspy.noConflict = function () { return $.fn.scrollspy = old, this }, $(window).on("load", function () { $('[data-spy="scroll"]').each(function () { var $spy = $(this); $spy.scrollspy($spy.data()) }) }) }(window.afjQuery), !function ($) { "use strict"; var Tab = function (element) { this.element = $(element) }; Tab.prototype = { constructor: Tab, show: function () { var previous, $target, e, $this = this.element, $ul = $this.closest("ul:not(.dropdown-menu)"), selector = $this.attr("data-target"); selector || (selector = $this.attr("href"), selector = selector && selector.replace(/.*(?=#[^\s]*$)/, "")), $this.parent("li").hasClass("active") || (previous = $ul.find(".active:last a")[0], e = $.Event("show", { relatedTarget: previous }), $this.trigger(e), e.isDefaultPrevented() || ($target = $(selector), this.activate($this.parent("li"), $ul), this.activate($target, $target.parent(), function () { $this.trigger({ type: "shown", relatedTarget: previous }) }))) }, activate: function (element, container, callback) { function next() { $active.removeClass("active").find("> .dropdown-menu > .active").removeClass("active"), element.addClass("active"), transition ? (element[0].offsetWidth, element.addClass("in")) : element.removeClass("fade"), element.parent(".dropdown-menu") && element.closest("li.dropdown").addClass("active"), callback && callback() } var $active = container.find("> .active"), transition = callback && $.support.transition && $active.hasClass("fade"); transition ? $active.one($.support.transition.end, next) : next(), $active.removeClass("in") } }; var old = $.fn.tab; $.fn.tab = function (option) { return this.each(function () { var $this = $(this), data = $this.data("tab"); data || $this.data("tab", data = new Tab(this)), "string" == typeof option && data[option]() }) }, $.fn.tab.Constructor = Tab, $.fn.tab.noConflict = function () { return $.fn.tab = old, this }, $(document).on("click.tab.data-api", '[data-toggle="tab"], [data-toggle="pill"]', function (e) { e.preventDefault(), $(this).tab("show") }) }(window.afjQuery), !function ($) { "use strict"; var Typeahead = function (element, options) { this.$element = $(element), this.options = $.extend({}, $.fn.typeahead.defaults, options), this.matcher = this.options.matcher || this.matcher, this.sorter = this.options.sorter || this.sorter, this.highlighter = this.options.highlighter || this.highlighter, this.updater = this.options.updater || this.updater, this.source = this.options.source, this.$menu = $(this.options.menu), this.shown = !1, this.listen() }; Typeahead.prototype = { constructor: Typeahead, select: function () { var val = this.$menu.find(".active").attr("data-value"); return this.$element.val(this.updater(val)).change(), this.hide() }, updater: function (item) { return item }, show: function () { var pos = $.extend({}, this.$element.position(), { height: this.$element[0].offsetHeight }); return this.$menu.insertAfter(this.$element).css({ top: pos.top + pos.height, left: pos.left }).show(), this.shown = !0, this }, hide: function () { return this.$menu.hide(), this.shown = !1, this }, lookup: function () { var items; return this.query = this.$element.val(), !this.query || this.query.length < this.options.minLength ? this.shown ? this.hide() : this : (items = $.isFunction(this.source) ? this.source(this.query, $.proxy(this.process, this)) : this.source, items ? this.process(items) : this) }, process: function (items) { var that = this; return items = $.grep(items, function (item) { return that.matcher(item) }), items = this.sorter(items), items.length ? this.render(items.slice(0, this.options.items)).show() : this.shown ? this.hide() : this }, matcher: function (item) { return ~item.toLowerCase().indexOf(this.query.toLowerCase()) }, sorter: function (items) { for (var item, beginswith = [], caseSensitive = [], caseInsensitive = []; item = items.shift() ;) item.toLowerCase().indexOf(this.query.toLowerCase()) ? ~item.indexOf(this.query) ? caseSensitive.push(item) : caseInsensitive.push(item) : beginswith.push(item); return beginswith.concat(caseSensitive, caseInsensitive) }, highlighter: function (item) { var query = this.query.replace(/[\-\[\]{}()*+?.,\\\^$|#\s]/g, "\\$&"); return item.replace(RegExp("(" + query + ")", "ig"), function ($1, match) { return "<strong>" + match + "</strong>" }) }, render: function (items) { var that = this; return items = $(items).map(function (i, item) { return i = $(that.options.item).attr("data-value", item), i.find("a").html(that.highlighter(item)), i[0] }), items.first().addClass("active"), this.$menu.html(items), this }, next: function () { var active = this.$menu.find(".active").removeClass("active"), next = active.next(); next.length || (next = $(this.$menu.find("li")[0])), next.addClass("active") }, prev: function () { var active = this.$menu.find(".active").removeClass("active"), prev = active.prev(); prev.length || (prev = this.$menu.find("li").last()), prev.addClass("active") }, listen: function () { this.$element.on("blur", $.proxy(this.blur, this)).on("keypress", $.proxy(this.keypress, this)).on("keyup", $.proxy(this.keyup, this)), this.eventSupported("keydown") && this.$element.on("keydown", $.proxy(this.keydown, this)), this.$menu.on("click", $.proxy(this.click, this)).on("mouseenter", "li", $.proxy(this.mouseenter, this)) }, eventSupported: function (eventName) { var isSupported = eventName in this.$element; return isSupported || (this.$element.setAttribute(eventName, "return;"), isSupported = "function" == typeof this.$element[eventName]), isSupported }, move: function (e) { if (this.shown) { switch (e.keyCode) { case 9: case 13: case 27: e.preventDefault(); break; case 38: e.preventDefault(), this.prev(); break; case 40: e.preventDefault(), this.next() } e.stopPropagation() } }, keydown: function (e) { this.suppressKeyPressRepeat = ~$.inArray(e.keyCode, [40, 38, 9, 13, 27]), this.move(e) }, keypress: function (e) { this.suppressKeyPressRepeat || this.move(e) }, keyup: function (e) { switch (e.keyCode) { case 40: case 38: case 16: case 17: case 18: break; case 9: case 13: if (!this.shown) return; this.select(); break; case 27: if (!this.shown) return; this.hide(); break; default: this.lookup() } e.stopPropagation(), e.preventDefault() }, blur: function () { var that = this; setTimeout(function () { that.hide() }, 150) }, click: function (e) { e.stopPropagation(), e.preventDefault(), this.select() }, mouseenter: function (e) { this.$menu.find(".active").removeClass("active"), $(e.currentTarget).addClass("active") } }; var old = $.fn.typeahead; $.fn.typeahead = function (option) { return this.each(function () { var $this = $(this), data = $this.data("typeahead"), options = "object" == typeof option && option; data || $this.data("typeahead", data = new Typeahead(this, options)), "string" == typeof option && data[option]() }) }, $.fn.typeahead.defaults = { source: [], items: 8, menu: '<ul class="typeahead dropdown-menu"></ul>', item: '<li><a href="#"></a></li>', minLength: 1 }, $.fn.typeahead.Constructor = Typeahead, $.fn.typeahead.noConflict = function () { return $.fn.typeahead = old, this }, $(document).on("focus.typeahead.data-api", '[data-provide="typeahead"]', function (e) { var $this = $(this); $this.data("typeahead") || (e.preventDefault(), $this.typeahead($this.data())) }) }(window.afjQuery), !function ($) { "use strict"; var Affix = function (element, options) { this.options = $.extend({}, $.fn.affix.defaults, options), this.$window = $(window).on("scroll.affix.data-api", $.proxy(this.checkPosition, this)).on("click.affix.data-api", $.proxy(function () { setTimeout($.proxy(this.checkPosition, this), 1) }, this)), this.$element = $(element), this.checkPosition() }; Affix.prototype.checkPosition = function () { if (this.$element.is(":visible")) { var affix, scrollHeight = $(document).height(), scrollTop = this.$window.scrollTop(), position = this.$element.offset(), offset = this.options.offset, offsetBottom = offset.bottom, offsetTop = offset.top, reset = "affix affix-top affix-bottom"; "object" != typeof offset && (offsetBottom = offsetTop = offset), "function" == typeof offsetTop && (offsetTop = offset.top()), "function" == typeof offsetBottom && (offsetBottom = offset.bottom()), affix = null != this.unpin && scrollTop + this.unpin <= position.top ? !1 : null != offsetBottom && position.top + this.$element.height() >= scrollHeight - offsetBottom ? "bottom" : null != offsetTop && offsetTop >= scrollTop ? "top" : !1, this.affixed !== affix && (this.affixed = affix, this.unpin = "bottom" == affix ? position.top - scrollTop : null, this.$element.removeClass(reset).addClass("affix" + (affix ? "-" + affix : ""))) } }; var old = $.fn.affix; $.fn.affix = function (option) { return this.each(function () { var $this = $(this), data = $this.data("affix"), options = "object" == typeof option && option; data || $this.data("affix", data = new Affix(this, options)), "string" == typeof option && data[option]() }) }, $.fn.affix.Constructor = Affix, $.fn.affix.defaults = { offset: 0 }, $.fn.affix.noConflict = function () { return $.fn.affix = old, this }, $(window).on("load", function () { $('[data-spy="affix"]').each(function () { var $spy = $(this), data = $spy.data(); data.offset = data.offset || {}, data.offsetBottom && (data.offset.bottom = data.offsetBottom), data.offsetTop && (data.offset.top = data.offsetTop), $spy.affix(data) }) }) }(window.afjQuery);

/*! jQuery Validation Plugin - v1.11.0 - 2/4/2013
* https://github.com/jzaefferer/jquery-validation
* Copyright (c) 2013 Jörn Zaefferer; Licensed MIT */


(function ($) {

    $.extend($.fn, {
        // http://docs.jquery.com/Plugins/Validation/validate
        validate: function (options) {

            // if nothing is selected, return nothing; can't chain anyway
            if (!this.length) {
                if (options && options.debug && window.console) {
                    console.warn("Nothing selected, can't validate, returning nothing.");
                }
                return;
            }

            // check if a validator for this form was already created
            var validator = $.data(this[0], "validator");
            if (validator) {
                return validator;
            }

            // Add novalidate tag if HTML5.
            this.attr("novalidate", "novalidate");

            validator = new $.validator(options, this[0]);
            $.data(this[0], "validator", validator);

            if (validator.settings.onsubmit) {

                this.validateDelegate(":submit", "click", function (event) {
                    if (validator.settings.submitHandler) {
                        validator.submitButton = event.target;
                    }
                    // allow suppressing validation by adding a cancel class to the submit button
                    if ($(event.target).hasClass("cancel")) {
                        validator.cancelSubmit = true;
                    }
                });

                // validate the form on submit
                this.submit(function (event) {
                    if (validator.settings.debug) {
                        // prevent form submit to be able to see console output
                        event.preventDefault();
                    }
                    function handle() {
                        var hidden;
                        if (validator.settings.submitHandler) {
                            if (validator.submitButton) {
                                // insert a hidden input as a replacement for the missing submit button
                                hidden = $("<input type='hidden'/>").attr("name", validator.submitButton.name).val(validator.submitButton.value).appendTo(validator.currentForm);
                            }
                            validator.settings.submitHandler.call(validator, validator.currentForm, event);
                            if (validator.submitButton) {
                                // and clean up afterwards; thanks to no-block-scope, hidden can be referenced
                                hidden.remove();
                            }
                            return false;
                        }
                        return true;
                    }

                    // prevent submit for invalid forms or custom submit handlers
                    if (validator.cancelSubmit) {
                        validator.cancelSubmit = false;
                        return handle();
                    }
                    if (validator.form()) {
                        if (validator.pendingRequest) {
                            validator.formSubmitted = true;
                            return false;
                        }
                        return handle();
                    } else {
                        validator.focusInvalid();
                        return false;
                    }
                });
            }

            return validator;
        },
        // http://docs.jquery.com/Plugins/Validation/valid
        valid: function () {
            if ($(this[0]).is("form")) {
                return this.validate().form();
            } else {
                var valid = true;
                var validator = $(this[0].form).validate();
                this.each(function () {
                    valid &= validator.element(this);
                });
                return valid;
            }
        },
        // attributes: space seperated list of attributes to retrieve and remove
        removeAttrs: function (attributes) {
            var result = {},
                $element = this;
            $.each(attributes.split(/\s/), function (index, value) {
                result[value] = $element.attr(value);
                $element.removeAttr(value);
            });
            return result;
        },
        // http://docs.jquery.com/Plugins/Validation/rules
        rules: function (command, argument) {
            var element = this[0];

            if (command) {
                var settings = $.data(element.form, "validator").settings;
                var staticRules = settings.rules;
                var existingRules = $.validator.staticRules(element);
                switch (command) {
                    case "add":
                        $.extend(existingRules, $.validator.normalizeRule(argument));
                        delete existingRules.messages; // we really don't want messages to come up as a rule
                        staticRules[element.name] = existingRules;
                        if (argument.messages) {
                            settings.messages[element.name] = $.extend(settings.messages[element.name], argument.messages);
                        }
                        break;
                    case "remove":
                        if (!argument) {
                            delete staticRules[element.name];
                            return existingRules;
                        }
                        var filtered = {};
                        $.each(argument.split(/\s/), function (index, method) {
                            filtered[method] = existingRules[method];
                            delete existingRules[method];
                        });
                        return filtered;
                }
            }

            var data = $.validator.normalizeRules(
            $.extend(
                {},
                $.validator.classRules(element),
                $.validator.attributeRules(element),
                $.validator.dataRules(element),
                $.validator.staticRules(element)
            ), element);

            // make sure required is at front
            if (data.required) {
                var param = data.required;
                delete data.required;
                data = $.extend({ required: param }, data);
            }

            return data;
        }
    });

    // Custom selectors
    $.extend($.expr[":"], {
        // http://docs.jquery.com/Plugins/Validation/blank
        blank: function (a) { return !$.trim("" + a.value); },
        // http://docs.jquery.com/Plugins/Validation/filled
        filled: function (a) { return !!$.trim("" + a.value); },
        // http://docs.jquery.com/Plugins/Validation/unchecked
        unchecked: function (a) { return !a.checked; }
    });

    // constructor for validator
    $.validator = function (options, form) {
        this.settings = $.extend(true, {}, $.validator.defaults, options);
        this.currentForm = form;
        this.init();
    };

    $.validator.format = function (source, params) {
        if (arguments.length === 1) {
            return function () {
                var args = $.makeArray(arguments);
                args.unshift(source);
                return $.validator.format.apply(this, args);
            };
        }
        if (arguments.length > 2 && params.constructor !== Array) {
            params = $.makeArray(arguments).slice(1);
        }
        if (params.constructor !== Array) {
            params = [params];
        }
        $.each(params, function (i, n) {
            source = source.replace(new RegExp("\\{" + i + "\\}", "g"), function () {
                return n;
            });
        });
        return source;
    };

    $.extend($.validator, {

        defaults: {
            messages: {},
            groups: {},
            rules: {},
            errorClass: "error",
            validClass: "valid",
            errorElement: "label",
            focusInvalid: true,
            errorContainer: $([]),
            errorLabelContainer: $([]),
            onsubmit: true,
            ignore: ":hidden",
            ignoreTitle: false,
            onfocusin: function (element, event) {
                this.lastActive = element;

                // hide error label and remove error class on focus if enabled
                if (this.settings.focusCleanup && !this.blockFocusCleanup) {
                    if (this.settings.unhighlight) {
                        this.settings.unhighlight.call(this, element, this.settings.errorClass, this.settings.validClass);
                    }
                    this.addWrapper(this.errorsFor(element)).hide();
                }
            },
            onfocusout: function (element, event) {
                if (!this.checkable(element) && (element.name in this.submitted || !this.optional(element))) {
                    this.element(element);
                }
            },
            onkeyup: function (element, event) {
                if (event.which === 9 && this.elementValue(element) === "") {
                    return;
                } else if (element.name in this.submitted || element === this.lastElement) {
                    this.element(element);
                }
            },
            onclick: function (element, event) {
                // click on selects, radiobuttons and checkboxes
                if (element.name in this.submitted) {
                    this.element(element);
                }
                    // or option elements, check parent select in that case
                else if (element.parentNode.name in this.submitted) {
                    this.element(element.parentNode);
                }
            },
            highlight: function (element, errorClass, validClass) {
                if (element.type === "radio") {
                    this.findByName(element.name).addClass(errorClass).removeClass(validClass);
                } else {
                    $(element).addClass(errorClass).removeClass(validClass);
                }
            },
            unhighlight: function (element, errorClass, validClass) {
                if (element.type === "radio") {
                    this.findByName(element.name).removeClass(errorClass).addClass(validClass);
                } else {
                    $(element).removeClass(errorClass).addClass(validClass);
                }
            }
        },

        // http://docs.jquery.com/Plugins/Validation/Validator/setDefaults
        setDefaults: function (settings) {
            $.extend($.validator.defaults, settings);
        },

        messages: {
            required: "This field is required.",
            remote: "Please fix this field.",
            email: "Please enter a valid email address.",
            url: "Please enter a valid URL.",
            date: "Please enter a valid date.",
            dateISO: "Please enter a valid date (ISO).",
            number: "Please enter a valid number.",
            digits: "Please enter only digits.",
            creditcard: "Please enter a valid credit card number.",
            equalTo: "Please enter the same value again.",
            maxlength: $.validator.format("Please enter no more than {0} characters."),
            minlength: $.validator.format("Please enter at least {0} characters."),
            rangelength: $.validator.format("Please enter a value between {0} and {1} characters long."),
            range: $.validator.format("Please enter a value between {0} and {1}."),
            max: $.validator.format("Please enter a value less than or equal to {0}."),
            min: $.validator.format("Please enter a value greater than or equal to {0}.")
        },

        autoCreateRanges: false,

        prototype: {

            init: function () {
                this.labelContainer = $(this.settings.errorLabelContainer);
                this.errorContext = this.labelContainer.length && this.labelContainer || $(this.currentForm);
                this.containers = $(this.settings.errorContainer).add(this.settings.errorLabelContainer);
                this.submitted = {};
                this.valueCache = {};
                this.pendingRequest = 0;
                this.pending = {};
                this.invalid = {};
                this.reset();

                var groups = (this.groups = {});
                $.each(this.settings.groups, function (key, value) {
                    if (typeof value === "string") {
                        value = value.split(/\s/);
                    }
                    $.each(value, function (index, name) {
                        groups[name] = key;
                    });
                });
                var rules = this.settings.rules;
                $.each(rules, function (key, value) {
                    rules[key] = $.validator.normalizeRule(value);
                });

                function delegate(event) {
                    var validator = $.data(this[0].form, "validator"),
                        eventType = "on" + event.type.replace(/^validate/, "");
                    if (validator.settings[eventType]) {
                        validator.settings[eventType].call(validator, this[0], event);
                    }
                }
                $(this.currentForm)
                    .validateDelegate(":text, [type='password'], [type='file'], select, textarea, " +
                        "[type='number'], [type='search'] ,[type='tel'], [type='url'], " +
                        "[type='email'], [type='datetime'], [type='date'], [type='month'], " +
                        "[type='week'], [type='time'], [type='datetime-local'], " +
                        "[type='range'], [type='color'] ",
                        "focusin focusout keyup", delegate)
                    .validateDelegate("[type='radio'], [type='checkbox'], select, option", "click", delegate);

                if (this.settings.invalidHandler) {
                    $(this.currentForm).bind("invalid-form.validate", this.settings.invalidHandler);
                }
            },

            // http://docs.jquery.com/Plugins/Validation/Validator/form
            form: function () {
                this.checkForm();
                $.extend(this.submitted, this.errorMap);
                this.invalid = $.extend({}, this.errorMap);
                if (!this.valid()) {
                    $(this.currentForm).triggerHandler("invalid-form", [this]);
                }
                this.showErrors();
                return this.valid();
            },

            checkForm: function () {
                this.prepareForm();
                for (var i = 0, elements = (this.currentElements = this.elements()) ; elements[i]; i++) {
                    this.check(elements[i]);
                }
                return this.valid();
            },

            // http://docs.jquery.com/Plugins/Validation/Validator/element
            element: function (element) {
                element = this.validationTargetFor(this.clean(element));
                this.lastElement = element;
                this.prepareElement(element);
                this.currentElements = $(element);
                var result = this.check(element) !== false;
                if (result) {
                    delete this.invalid[element.name];
                } else {
                    this.invalid[element.name] = true;
                }
                if (!this.numberOfInvalids()) {
                    // Hide error containers on last error
                    this.toHide = this.toHide.add(this.containers);
                }
                this.showErrors();
                return result;
            },

            // http://docs.jquery.com/Plugins/Validation/Validator/showErrors
            showErrors: function (errors) {
                if (errors) {
                    // add items to error list and map
                    $.extend(this.errorMap, errors);
                    this.errorList = [];
                    for (var name in errors) {
                        this.errorList.push({
                            message: errors[name],
                            element: this.findByName(name)[0]
                        });
                    }
                    // remove items from success list
                    this.successList = $.grep(this.successList, function (element) {
                        return !(element.name in errors);
                    });
                }
                if (this.settings.showErrors) {
                    this.settings.showErrors.call(this, this.errorMap, this.errorList);
                } else {
                    this.defaultShowErrors();
                }
            },

            // http://docs.jquery.com/Plugins/Validation/Validator/resetForm
            resetForm: function () {
                if ($.fn.resetForm) {
                    $(this.currentForm).resetForm();
                }
                this.submitted = {};
                this.lastElement = null;
                this.prepareForm();
                this.hideErrors();
                this.elements().removeClass(this.settings.errorClass).removeData("previousValue");
            },

            numberOfInvalids: function () {
                return this.objectLength(this.invalid);
            },

            objectLength: function (obj) {
                var count = 0;
                for (var i in obj) {
                    count++;
                }
                return count;
            },

            hideErrors: function () {
                this.addWrapper(this.toHide).hide();
            },

            valid: function () {
                return this.size() === 0;
            },

            size: function () {
                return this.errorList.length;
            },

            focusInvalid: function () {
                if (this.settings.focusInvalid) {
                    try {
                        $(this.findLastActive() || this.errorList.length && this.errorList[0].element || [])
                        .filter(":visible")
                        .focus()
                        // manually trigger focusin event; without it, focusin handler isn't called, findLastActive won't have anything to find
                        .trigger("focusin");
                    } catch (e) {
                        // ignore IE throwing errors when focusing hidden elements
                    }
                }
            },

            findLastActive: function () {
                var lastActive = this.lastActive;
                return lastActive && $.grep(this.errorList, function (n) {
                    return n.element.name === lastActive.name;
                }).length === 1 && lastActive;
            },

            elements: function () {
                var validator = this,
                    rulesCache = {};

                // select all valid inputs inside the form (no submit or reset buttons)
                return $(this.currentForm)
                .find("input, select, textarea")
                .not(":submit, :reset, :image, [disabled]")
                .not(this.settings.ignore)
                .filter(function () {
                    if (!this.name && validator.settings.debug && window.console) {
                        console.error("%o has no name assigned", this);
                    }

                    // select only the first element for each name, and only those with rules specified
                    if (this.name in rulesCache || !validator.objectLength($(this).rules())) {
                        return false;
                    }

                    rulesCache[this.name] = true;
                    return true;
                });
            },

            clean: function (selector) {
                return $(selector)[0];
            },

            errors: function () {
                var errorClass = this.settings.errorClass.replace(" ", ".");
                return $(this.settings.errorElement + "." + errorClass, this.errorContext);
            },

            reset: function () {
                this.successList = [];
                this.errorList = [];
                this.errorMap = {};
                this.toShow = $([]);
                this.toHide = $([]);
                this.currentElements = $([]);
            },

            prepareForm: function () {
                this.reset();
                this.toHide = this.errors().add(this.containers);
            },

            prepareElement: function (element) {
                this.reset();
                this.toHide = this.errorsFor(element);
            },

            elementValue: function (element) {
                var type = $(element).attr("type"),
                    val = $(element).val();

                if (type === "radio" || type === "checkbox") {
                    return $("input[name='" + $(element).attr("name") + "']:checked").val();
                }

                if (typeof val === "string") {
                    return val.replace(/\r/g, "");
                }
                return val;
            },

            check: function (element) {
                element = this.validationTargetFor(this.clean(element));

                var rules = $(element).rules();
                var dependencyMismatch = false;
                var val = this.elementValue(element);
                var result;

                for (var method in rules) {
                    var rule = { method: method, parameters: rules[method] };
                    try {

                        result = $.validator.methods[method].call(this, val, element, rule.parameters);

                        // if a method indicates that the field is optional and therefore valid,
                        // don't mark it as valid when there are no other rules
                        if (result === "dependency-mismatch") {
                            dependencyMismatch = true;
                            continue;
                        }
                        dependencyMismatch = false;

                        if (result === "pending") {
                            this.toHide = this.toHide.not(this.errorsFor(element));
                            return;
                        }

                        if (!result) {
                            this.formatAndAdd(element, rule);
                            return false;
                        }
                    } catch (e) {
                        if (this.settings.debug && window.console) {
                            console.log("Exception occured when checking element " + element.id + ", check the '" + rule.method + "' method.", e);
                        }
                        throw e;
                    }
                }
                if (dependencyMismatch) {
                    return;
                }
                if (this.objectLength(rules)) {
                    this.successList.push(element);
                }
                return true;
            },

            // return the custom message for the given element and validation method
            // specified in the element's HTML5 data attribute
            customDataMessage: function (element, method) {
                return $(element).data("msg-" + method.toLowerCase()) || (element.attributes && $(element).attr("data-msg-" + method.toLowerCase()));
            },

            // return the custom message for the given element name and validation method
            customMessage: function (name, method) {
                var m = this.settings.messages[name];
                return m && (m.constructor === String ? m : m[method]);
            },

            // return the first defined argument, allowing empty strings
            findDefined: function () {
                for (var i = 0; i < arguments.length; i++) {
                    if (arguments[i] !== undefined) {
                        return arguments[i];
                    }
                }
                return undefined;
            },

            defaultMessage: function (element, method) {
                return this.findDefined(
                    this.customMessage(element.name, method),
                    this.customDataMessage(element, method),
                    // title is never undefined, so handle empty string as undefined
                    !this.settings.ignoreTitle && element.title || undefined,
                    $.validator.messages[method],
                    "<strong>Warning: No message defined for " + element.name + "</strong>"
                );
            },

            formatAndAdd: function (element, rule) {
                var message = this.defaultMessage(element, rule.method),
                    theregex = /\$?\{(\d+)\}/g;
                if (typeof message === "function") {
                    message = message.call(this, rule.parameters, element);
                } else if (theregex.test(message)) {
                    message = $.validator.format(message.replace(theregex, "{$1}"), rule.parameters);
                }
                this.errorList.push({
                    message: message,
                    element: element
                });

                this.errorMap[element.name] = message;
                this.submitted[element.name] = message;
            },

            addWrapper: function (toToggle) {
                if (this.settings.wrapper) {
                    toToggle = toToggle.add(toToggle.parent(this.settings.wrapper));
                }
                return toToggle;
            },

            defaultShowErrors: function () {
                var i, elements;
                for (i = 0; this.errorList[i]; i++) {
                    var error = this.errorList[i];
                    if (this.settings.highlight) {
                        this.settings.highlight.call(this, error.element, this.settings.errorClass, this.settings.validClass);
                    }
                    this.showLabel(error.element, error.message);
                }
                if (this.errorList.length) {
                    this.toShow = this.toShow.add(this.containers);
                }
                if (this.settings.success) {
                    for (i = 0; this.successList[i]; i++) {
                        this.showLabel(this.successList[i]);
                    }
                }
                if (this.settings.unhighlight) {
                    for (i = 0, elements = this.validElements() ; elements[i]; i++) {
                        this.settings.unhighlight.call(this, elements[i], this.settings.errorClass, this.settings.validClass);
                    }
                }
                this.toHide = this.toHide.not(this.toShow);
                this.hideErrors();
                this.addWrapper(this.toShow).show();
            },

            validElements: function () {
                return this.currentElements.not(this.invalidElements());
            },

            invalidElements: function () {
                return $(this.errorList).map(function () {
                    return this.element;
                });
            },

            showLabel: function (element, message) {
                var label = this.errorsFor(element);
                if (label.length) {
                    // refresh error/success class
                    label.removeClass(this.settings.validClass).addClass(this.settings.errorClass);
                    // replace message on existing label
                    label.html(message);
                } else {
                    // create label
                    label = $("<" + this.settings.errorElement + ">")
                        .attr("for", this.idOrName(element))
                        .addClass(this.settings.errorClass)
                        .html(message || "");
                    if (this.settings.wrapper) {
                        // make sure the element is visible, even in IE
                        // actually showing the wrapped element is handled elsewhere
                        label = label.hide().show().wrap("<" + this.settings.wrapper + "/>").parent();
                    }
                    if (!this.labelContainer.append(label).length) {
                        if (this.settings.errorPlacement) {
                            this.settings.errorPlacement(label, $(element));
                        } else {
                            label.insertAfter(element);
                        }
                    }
                }
                if (!message && this.settings.success) {
                    label.text("");
                    if (typeof this.settings.success === "string") {
                        label.addClass(this.settings.success);
                    } else {
                        this.settings.success(label, element);
                    }
                }
                this.toShow = this.toShow.add(label);
            },

            errorsFor: function (element) {
                var name = this.idOrName(element);
                return this.errors().filter(function () {
                    return $(this).attr("for") === name;
                });
            },

            idOrName: function (element) {
                return this.groups[element.name] || (this.checkable(element) ? element.name : element.id || element.name);
            },

            validationTargetFor: function (element) {
                // if radio/checkbox, validate first element in group instead
                if (this.checkable(element)) {
                    element = this.findByName(element.name).not(this.settings.ignore)[0];
                }
                return element;
            },

            checkable: function (element) {
                return (/radio|checkbox/i).test(element.type);
            },

            findByName: function (name) {
                return $(this.currentForm).find("[name='" + name + "']");
            },

            getLength: function (value, element) {
                switch (element.nodeName.toLowerCase()) {
                    case "select":
                        return $("option:selected", element).length;
                    case "input":
                        if (this.checkable(element)) {
                            return this.findByName(element.name).filter(":checked").length;
                        }
                }
                return value.length;
            },

            depend: function (param, element) {
                return this.dependTypes[typeof param] ? this.dependTypes[typeof param](param, element) : true;
            },

            dependTypes: {
                "boolean": function (param, element) {
                    return param;
                },
                "string": function (param, element) {
                    return !!$(param, element.form).length;
                },
                "function": function (param, element) {
                    return param(element);
                }
            },

            optional: function (element) {
                var val = this.elementValue(element);
                return !$.validator.methods.required.call(this, val, element) && "dependency-mismatch";
            },

            startRequest: function (element) {
                if (!this.pending[element.name]) {
                    this.pendingRequest++;
                    this.pending[element.name] = true;
                }
            },

            stopRequest: function (element, valid) {
                this.pendingRequest--;
                // sometimes synchronization fails, make sure pendingRequest is never < 0
                if (this.pendingRequest < 0) {
                    this.pendingRequest = 0;
                }
                delete this.pending[element.name];
                if (valid && this.pendingRequest === 0 && this.formSubmitted && this.form()) {
                    $(this.currentForm).submit();
                    this.formSubmitted = false;
                } else if (!valid && this.pendingRequest === 0 && this.formSubmitted) {
                    $(this.currentForm).triggerHandler("invalid-form", [this]);
                    this.formSubmitted = false;
                }
            },

            previousValue: function (element) {
                return $.data(element, "previousValue") || $.data(element, "previousValue", {
                    old: null,
                    valid: true,
                    message: this.defaultMessage(element, "remote")
                });
            }

        },

        classRuleSettings: {
            required: { required: true },
            email: { email: true },
            url: { url: true },
            date: { date: true },
            dateISO: { dateISO: true },
            number: { number: true },
            digits: { digits: true },
            creditcard: { creditcard: true }
        },

        addClassRules: function (className, rules) {
            if (className.constructor === String) {
                this.classRuleSettings[className] = rules;
            } else {
                $.extend(this.classRuleSettings, className);
            }
        },

        classRules: function (element) {
            var rules = {};
            var classes = $(element).attr("class");
            if (classes) {
                $.each(classes.split(" "), function () {
                    if (this in $.validator.classRuleSettings) {
                        $.extend(rules, $.validator.classRuleSettings[this]);
                    }
                });
            }
            return rules;
        },

        attributeRules: function (element) {
            var rules = {};
            var $element = $(element);

            for (var method in $.validator.methods) {
                var value;

                // support for <input required> in both html5 and older browsers
                if (method === "required") {
                    value = $element.get(0).getAttribute(method);
                    // Some browsers return an empty string for the required attribute
                    // and non-HTML5 browsers might have required="" markup
                    if (value === "") {
                        value = true;
                    }
                    // force non-HTML5 browsers to return bool
                    value = !!value;
                } else {
                    value = $element.attr(method);
                }

                if (value) {
                    rules[method] = value;
                } else if ($element[0].getAttribute("type") === method) {
                    rules[method] = true;
                }
            }

            // maxlength may be returned as -1, 2147483647 (IE) and 524288 (safari) for text inputs
            if (rules.maxlength && /-1|2147483647|524288/.test(rules.maxlength)) {
                delete rules.maxlength;
            }

            return rules;
        },

        dataRules: function (element) {
            var method, value,
                rules = {}, $element = $(element);
            for (method in $.validator.methods) {
                value = $element.data("rule-" + method.toLowerCase());
                if (value !== undefined) {
                    rules[method] = value;
                }
            }
            return rules;
        },

        staticRules: function (element) {
            var rules = {};
            var validator = $.data(element.form, "validator");
            if (validator.settings.rules) {
                rules = $.validator.normalizeRule(validator.settings.rules[element.name]) || {};
            }
            return rules;
        },

        normalizeRules: function (rules, element) {
            // handle dependency check
            $.each(rules, function (prop, val) {
                // ignore rule when param is explicitly false, eg. required:false
                if (val === false) {
                    delete rules[prop];
                    return;
                }
                if (val.param || val.depends) {
                    var keepRule = true;
                    switch (typeof val.depends) {
                        case "string":
                            keepRule = !!$(val.depends, element.form).length;
                            break;
                        case "function":
                            keepRule = val.depends.call(element, element);
                            break;
                    }
                    if (keepRule) {
                        rules[prop] = val.param !== undefined ? val.param : true;
                    } else {
                        delete rules[prop];
                    }
                }
            });

            // evaluate parameters
            $.each(rules, function (rule, parameter) {
                rules[rule] = $.isFunction(parameter) ? parameter(element) : parameter;
            });

            // clean number parameters
            $.each(['minlength', 'maxlength'], function () {
                if (rules[this]) {
                    rules[this] = Number(rules[this]);
                }
            });
            $.each(['rangelength'], function () {
                var parts;
                if (rules[this]) {
                    if ($.isArray(rules[this])) {
                        rules[this] = [Number(rules[this][0]), Number(rules[this][1])];
                    } else if (typeof rules[this] === "string") {
                        parts = rules[this].split(/[\s,]+/);
                        rules[this] = [Number(parts[0]), Number(parts[1])];
                    }
                }
            });

            if ($.validator.autoCreateRanges) {
                // auto-create ranges
                if (rules.min && rules.max) {
                    rules.range = [rules.min, rules.max];
                    delete rules.min;
                    delete rules.max;
                }
                if (rules.minlength && rules.maxlength) {
                    rules.rangelength = [rules.minlength, rules.maxlength];
                    delete rules.minlength;
                    delete rules.maxlength;
                }
            }

            return rules;
        },

        // Converts a simple string to a {string: true} rule, e.g., "required" to {required:true}
        normalizeRule: function (data) {
            if (typeof data === "string") {
                var transformed = {};
                $.each(data.split(/\s/), function () {
                    transformed[this] = true;
                });
                data = transformed;
            }
            return data;
        },

        // http://docs.jquery.com/Plugins/Validation/Validator/addMethod
        addMethod: function (name, method, message) {
            $.validator.methods[name] = method;
            $.validator.messages[name] = message !== undefined ? message : $.validator.messages[name];
            if (method.length < 3) {
                $.validator.addClassRules(name, $.validator.normalizeRule(name));
            }
        },

        methods: {

            // http://docs.jquery.com/Plugins/Validation/Methods/required
            required: function (value, element, param) {
                // check if dependency is met
                if (!this.depend(param, element)) {
                    return "dependency-mismatch";
                }
                if (element.nodeName.toLowerCase() === "select") {
                    // could be an array for select-multiple or a string, both are fine this way
                    var val = $(element).val();
                    return val && val.length > 0;
                }
                if (this.checkable(element)) {
                    return this.getLength(value, element) > 0;
                }
                return $.trim(value).length > 0;
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/remote
            remote: function (value, element, param) {
                if (this.optional(element)) {
                    return "dependency-mismatch";
                }

                var previous = this.previousValue(element);
                if (!this.settings.messages[element.name]) {
                    this.settings.messages[element.name] = {};
                }
                previous.originalMessage = this.settings.messages[element.name].remote;
                this.settings.messages[element.name].remote = previous.message;

                param = typeof param === "string" && { url: param } || param;

                if (previous.old === value) {
                    return previous.valid;
                }

                previous.old = value;
                var validator = this;
                this.startRequest(element);
                var data = {};
                data[element.name] = value;
                $.ajax($.extend(true, {
                    url: param,
                    mode: "abort",
                    port: "validate" + element.name,
                    dataType: "json",
                    data: data,
                    success: function (response) {
                        validator.settings.messages[element.name].remote = previous.originalMessage;
                        var valid = response === true || response === "true";
                        if (valid) {
                            var submitted = validator.formSubmitted;
                            validator.prepareElement(element);
                            validator.formSubmitted = submitted;
                            validator.successList.push(element);
                            delete validator.invalid[element.name];
                            validator.showErrors();
                        } else {
                            var errors = {};
                            var message = response || validator.defaultMessage(element, "remote");
                            errors[element.name] = previous.message = $.isFunction(message) ? message(value) : message;
                            validator.invalid[element.name] = true;
                            validator.showErrors(errors);
                        }
                        previous.valid = valid;
                        validator.stopRequest(element, valid);
                    }
                }, param));
                return "pending";
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/minlength
            minlength: function (value, element, param) {
                var length = $.isArray(value) ? value.length : this.getLength($.trim(value), element);
                return this.optional(element) || length >= param;
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/maxlength
            maxlength: function (value, element, param) {
                var length = $.isArray(value) ? value.length : this.getLength($.trim(value), element);
                return this.optional(element) || length <= param;
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/rangelength
            rangelength: function (value, element, param) {
                var length = $.isArray(value) ? value.length : this.getLength($.trim(value), element);
                return this.optional(element) || (length >= param[0] && length <= param[1]);
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/min
            min: function (value, element, param) {
                return this.optional(element) || value >= param;
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/max
            max: function (value, element, param) {
                return this.optional(element) || value <= param;
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/range
            range: function (value, element, param) {
                return this.optional(element) || (value >= param[0] && value <= param[1]);
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/email
            email: function (value, element) {
                // contributed by Scott Gonzalez: http://projects.scottsplayground.com/email_address_validation/
                return this.optional(element) || /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$/i.test(value);
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/url
            url: function (value, element) {
                // contributed by Scott Gonzalez: http://projects.scottsplayground.com/iri/
                return this.optional(element) || /^(https?|s?ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i.test(value);
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/date
            date: function (value, element) {
                return this.optional(element) || !/Invalid|NaN/.test(new Date(value).toString());
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/dateISO
            dateISO: function (value, element) {
                return this.optional(element) || /^\d{4}[\/\-]\d{1,2}[\/\-]\d{1,2}$/.test(value);
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/number
            number: function (value, element) {
                return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$/.test(value);
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/digits
            digits: function (value, element) {
                return this.optional(element) || /^\d+$/.test(value);
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/creditcard
            // based on http://en.wikipedia.org/wiki/Luhn
            creditcard: function (value, element) {
                if (this.optional(element)) {
                    return "dependency-mismatch";
                }
                // accept only spaces, digits and dashes
                if (/[^0-9 \-]+/.test(value)) {
                    return false;
                }
                var nCheck = 0,
                    nDigit = 0,
                    bEven = false;

                value = value.replace(/\D/g, "");

                for (var n = value.length - 1; n >= 0; n--) {
                    var cDigit = value.charAt(n);
                    nDigit = parseInt(cDigit, 10);
                    if (bEven) {
                        if ((nDigit *= 2) > 9) {
                            nDigit -= 9;
                        }
                    }
                    nCheck += nDigit;
                    bEven = !bEven;
                }

                return (nCheck % 10) === 0;
            },

            // http://docs.jquery.com/Plugins/Validation/Methods/equalTo
            equalTo: function (value, element, param) {
                // bind to the blur event of the target in order to revalidate whenever the target field is updated
                // TODO find a way to bind the event just once, avoiding the unbind-rebind overhead
                var target = $(param);
                if (this.settings.onfocusout) {
                    target.unbind(".validate-equalTo").bind("blur.validate-equalTo", function () {
                        $(element).valid();
                    });
                }
                return value === target.val();
            }

        }

    });

    // deprecated, use $.validator.format instead
    $.format = $.validator.format;

}(afjQuery));

// ajax mode: abort
// usage: $.ajax({ mode: "abort"[, port: "uniqueport"]});
// if mode:"abort" is used, the previous request on that port (port can be undefined) is aborted via XMLHttpRequest.abort()
(function ($) {
    var pendingRequests = {};
    // Use a prefilter if available (1.5+)
    if ($.ajaxPrefilter) {
        $.ajaxPrefilter(function (settings, _, xhr) {
            var port = settings.port;
            if (settings.mode === "abort") {
                if (pendingRequests[port]) {
                    pendingRequests[port].abort();
                }
                pendingRequests[port] = xhr;
            }
        });
    } else {
        // Proxy ajax
        var ajax = $.ajax;
        $.ajax = function (settings) {
            var mode = ("mode" in settings ? settings : $.ajaxSettings).mode,
				port = ("port" in settings ? settings : $.ajaxSettings).port;
            if (mode === "abort") {
                if (pendingRequests[port]) {
                    pendingRequests[port].abort();
                }
                return (pendingRequests[port] = ajax.apply(this, arguments));
            }
            return ajax.apply(this, arguments);
        };
    }
}(afjQuery));

// provides delegate(type: String, delegate: Selector, handler: Callback) plugin for easier event delegation
// handler is only called when $(event.target).is(delegate), in the scope of the jquery-object for event.target
(function ($) {
    $.extend($.fn, {
        validateDelegate: function (delegate, type, handler) {
            return this.bind(type, function (event) {
                var target = $(event.target);
                if (target.is(delegate)) {
                    return handler.apply(target, arguments);
                }
            });
        }
    });
}(afjQuery));

/*yepnope1.5.x|WTFPL*/
(function (a, b, c) { function d(a) { return "[object Function]" == o.call(a) } function e(a) { return "string" == typeof a } function f() { } function g(a) { return !a || "loaded" == a || "complete" == a || "uninitialized" == a } function h() { var a = p.shift(); q = 1, a ? a.t ? m(function () { ("c" == a.t ? B.injectCss : B.injectJs)(a.s, 0, a.a, a.x, a.e, 1) }, 0) : (a(), h()) : q = 0 } function i(a, c, d, e, f, i, j) { function k(b) { if (!o && g(l.readyState) && (u.r = o = 1, !q && h(), l.onload = l.onreadystatechange = null, b)) { "img" != a && m(function () { t.removeChild(l) }, 50); for (var d in y[c]) y[c].hasOwnProperty(d) && y[c][d].onload() } } var j = j || B.errorTimeout, l = b.createElement(a), o = 0, r = 0, u = { t: d, s: c, e: f, a: i, x: j }; 1 === y[c] && (r = 1, y[c] = []), "object" == a ? l.data = c : (l.src = c, l.type = a), l.width = l.height = "0", l.onerror = l.onload = l.onreadystatechange = function () { k.call(this, r) }, p.splice(e, 0, u), "img" != a && (r || 2 === y[c] ? (t.insertBefore(l, s ? null : n), m(k, j)) : y[c].push(l)) } function j(a, b, c, d, f) { return q = 0, b = b || "j", e(a) ? i("c" == b ? v : u, a, b, this.i++, c, d, f) : (p.splice(this.i++, 0, a), 1 == p.length && h()), this } function k() { var a = B; return a.loader = { load: j, i: 0 }, a } var l = b.documentElement, m = a.setTimeout, n = b.getElementsByTagName("script")[0], o = {}.toString, p = [], q = 0, r = "MozAppearance" in l.style, s = r && !!b.createRange().compareNode, t = s ? l : n.parentNode, l = a.opera && "[object Opera]" == o.call(a.opera), l = !!b.attachEvent && !l, u = r ? "object" : l ? "script" : "img", v = l ? "script" : u, w = Array.isArray || function (a) { return "[object Array]" == o.call(a) }, x = [], y = {}, z = { timeout: function (a, b) { return b.length && (a.timeout = b[0]), a } }, A, B; B = function (a) { function b(a) { var a = a.split("!"), b = x.length, c = a.pop(), d = a.length, c = { url: c, origUrl: c, prefixes: a }, e, f, g; for (f = 0; f < d; f++) g = a[f].split("="), (e = z[g.shift()]) && (c = e(c, g)); for (f = 0; f < b; f++) c = x[f](c); return c } function g(a, e, f, g, h) { var i = b(a), j = i.autoCallback; i.url.split(".").pop().split("?").shift(), i.bypass || (e && (e = d(e) ? e : e[a] || e[g] || e[a.split("/").pop().split("?")[0]]), i.instead ? i.instead(a, e, f, g, h) : (y[i.url] ? i.noexec = !0 : y[i.url] = 1, f.load(i.url, i.forceCSS || !i.forceJS && "css" == i.url.split(".").pop().split("?").shift() ? "c" : c, i.noexec, i.attrs, i.timeout), (d(e) || d(j)) && f.load(function () { k(), e && e(i.origUrl, h, g), j && j(i.origUrl, h, g), y[i.url] = 2 }))) } function h(a, b) { function c(a, c) { if (a) { if (e(a)) c || (j = function () { var a = [].slice.call(arguments); k.apply(this, a), l() }), g(a, j, b, 0, h); else if (Object(a) === a) for (n in m = function () { var b = 0, c; for (c in a) a.hasOwnProperty(c) && b++; return b }(), a) a.hasOwnProperty(n) && (!c && !--m && (d(j) ? j = function () { var a = [].slice.call(arguments); k.apply(this, a), l() } : j[n] = function (a) { return function () { var b = [].slice.call(arguments); a && a.apply(this, b), l() } }(k[n])), g(a[n], j, b, n, h)) } else !c && l() } var h = !!a.test, i = a.load || a.both, j = a.callback || f, k = j, l = a.complete || f, m, n; c(h ? a.yep : a.nope, !!i), i && c(i) } var i, j, l = this.yepnope.loader; if (e(a)) g(a, 0, l, 0); else if (w(a)) for (i = 0; i < a.length; i++) j = a[i], e(j) ? g(j, 0, l, 0) : w(j) ? B(j) : Object(j) === j && h(j, l); else Object(a) === a && h(a, l) }, B.addPrefix = function (a, b) { z[a] = b }, B.addFilter = function (a) { x.push(a) }, B.errorTimeout = 1e4, null == b.readyState && b.addEventListener && (b.readyState = "loading", b.addEventListener("DOMContentLoaded", A = function () { b.removeEventListener("DOMContentLoaded", A, 0), b.readyState = "complete" }, 0)), a.yepnope = k(), a.yepnope.executeStack = h, a.yepnope.injectJs = function (a, c, d, e, i, j) { var k = b.createElement("script"), l, o, e = e || B.errorTimeout; k.src = a; for (o in d) k.setAttribute(o, d[o]); c = j ? h : c || f, k.onreadystatechange = k.onload = function () { !l && g(k.readyState) && (l = 1, c(), k.onload = k.onreadystatechange = null) }, m(function () { l || (l = 1, c(1)) }, e), i ? k.onload() : n.parentNode.insertBefore(k, n) }, a.yepnope.injectCss = function (a, c, d, e, g, i) { var e = b.createElement("link"), j, c = i ? h : c || f; e.href = a, e.rel = "stylesheet", e.type = "text/css"; for (j in d) e.setAttribute(j, d[j]); g || (n.parentNode.insertBefore(e, n), m(c, 0)) } })(this, document);


/*
	Masked Input plugin for jQuery
	Copyright (c) 2007-2013 Josh Bush (digitalbush.com)
	Licensed under the MIT license (http://digitalbush.com/projects/masked-input-plugin/#license)
	Version: 1.3.1
*/
(function (e) { function t() { var e = document.createElement("input"), t = "onpaste"; return e.setAttribute(t, ""), "function" == typeof e[t] ? "paste" : "input" } var n, a = t() + ".mask", r = navigator.userAgent, i = /iphone/i.test(r), o = /android/i.test(r); e.mask = { definitions: { 9: "[0-9]", a: "[A-Za-z]", "*": "[A-Za-z0-9]" }, dataName: "rawMaskFn", placeholder: "_" }, e.fn.extend({ caret: function (e, t) { var n; if (0 !== this.length && !this.is(":hidden")) return "number" == typeof e ? (t = "number" == typeof t ? t : e, this.each(function () { this.setSelectionRange ? this.setSelectionRange(e, t) : this.createTextRange && (n = this.createTextRange(), n.collapse(!0), n.moveEnd("character", t), n.moveStart("character", e), n.select()) })) : (this[0].setSelectionRange ? (e = this[0].selectionStart, t = this[0].selectionEnd) : document.selection && document.selection.createRange && (n = document.selection.createRange(), e = 0 - n.duplicate().moveStart("character", -1e5), t = e + n.text.length), { begin: e, end: t }) }, unmask: function () { return this.trigger("unmask") }, mask: function (t, r) { var c, l, s, u, f, h; return !t && this.length > 0 ? (c = e(this[0]), c.data(e.mask.dataName)()) : (r = e.extend({ placeholder: e.mask.placeholder, completed: null }, r), l = e.mask.definitions, s = [], u = h = t.length, f = null, e.each(t.split(""), function (e, t) { "?" == t ? (h--, u = e) : l[t] ? (s.push(RegExp(l[t])), null === f && (f = s.length - 1)) : s.push(null) }), this.trigger("unmask").each(function () { function c(e) { for (; h > ++e && !s[e];); return e } function d(e) { for (; --e >= 0 && !s[e];); return e } function m(e, t) { var n, a; if (!(0 > e)) { for (n = e, a = c(t) ; h > n; n++) if (s[n]) { if (!(h > a && s[n].test(R[a]))) break; R[n] = R[a], R[a] = r.placeholder, a = c(a) } b(), x.caret(Math.max(f, e)) } } function p(e) { var t, n, a, i; for (t = e, n = r.placeholder; h > t; t++) if (s[t]) { if (a = c(t), i = R[t], R[t] = n, !(h > a && s[a].test(i))) break; n = i } } function g(e) { var t, n, a, r = e.which; 8 === r || 46 === r || i && 127 === r ? (t = x.caret(), n = t.begin, a = t.end, 0 === a - n && (n = 46 !== r ? d(n) : a = c(n - 1), a = 46 === r ? c(a) : a), k(n, a), m(n, a - 1), e.preventDefault()) : 27 == r && (x.val(S), x.caret(0, y()), e.preventDefault()) } function v(t) { var n, a, i, l = t.which, u = x.caret(); t.ctrlKey || t.altKey || t.metaKey || 32 > l || l && (0 !== u.end - u.begin && (k(u.begin, u.end), m(u.begin, u.end - 1)), n = c(u.begin - 1), h > n && (a = String.fromCharCode(l), s[n].test(a) && (p(n), R[n] = a, b(), i = c(n), o ? setTimeout(e.proxy(e.fn.caret, x, i), 0) : x.caret(i), r.completed && i >= h && r.completed.call(x))), t.preventDefault()) } function k(e, t) { var n; for (n = e; t > n && h > n; n++) s[n] && (R[n] = r.placeholder) } function b() { x.val(R.join("")) } function y(e) { var t, n, a = x.val(), i = -1; for (t = 0, pos = 0; h > t; t++) if (s[t]) { for (R[t] = r.placeholder; pos++ < a.length;) if (n = a.charAt(pos - 1), s[t].test(n)) { R[t] = n, i = t; break } if (pos > a.length) break } else R[t] === a.charAt(pos) && t !== u && (pos++, i = t); return e ? b() : u > i + 1 ? (x.val(""), k(0, h)) : (b(), x.val(x.val().substring(0, i + 1))), u ? t : f } var x = e(this), R = e.map(t.split(""), function (e) { return "?" != e ? l[e] ? r.placeholder : e : void 0 }), S = x.val(); x.data(e.mask.dataName, function () { return e.map(R, function (e, t) { return s[t] && e != r.placeholder ? e : null }).join("") }), x.attr("readonly") || x.one("unmask", function () { x.unbind(".mask").removeData(e.mask.dataName) }).bind("focus.mask", function () { clearTimeout(n); var e; S = x.val(), e = y(), n = setTimeout(function () { b(), e == t.length ? x.caret(0, e) : x.caret(e) }, 10) }).bind("blur.mask", function () { y(), x.val() != S && x.change() }).bind("keydown.mask", g).bind("keypress.mask", v).bind(a, function () { setTimeout(function () { var e = y(!0); x.caret(e), r.completed && e == x.val().length && r.completed.call(x) }, 0) }), y() })) } }) })(afjQuery);


/* IE Placeholder fix
*/
(function ($) {
    // @todo Document this.
    $.extend($, {
        placeholder: {
            browser_supported: function () {
                return this._supported !== undefined ?
                  this._supported :
                  (this._supported = !!('placeholder' in $('<input type="text">')[0]));
            },
            shim: function (opts) {
                var config = {
                    color: '#888',
                    cls: 'placeholder',
                    selector: 'input[placeholder], textarea[placeholder]'
                };
                $.extend(config, opts);
                return !this.browser_supported() && $(config.selector)._placeholder_shim(config);
            }
        }
    });

    $.extend($.fn, {
        _placeholder_shim: function (config) {
            function calcPositionCss(target) {
                var op = $(target).offsetParent().offset();
                var ot = $(target).offset();

                return {
                    top: ot.top - op.top,
                    left: ot.left - op.left,
                    width: $(target).width()
                };
            }
            function adjustToResizing(label) {
                var $target = label.data('target');
                if (typeof $target !== "undefined") {
                    label.css(calcPositionCss($target));
                    $(window).one("resize", function () { adjustToResizing(label); });
                }
            }
            return this.each(function () {
                var $this = $(this);

                if ($this.is(':visible')) {

                    if ($this.data('placeholder')) {
                        var $ol = $this.data('placeholder');
                        $ol.css(calcPositionCss($this));
                        return true;
                    }

                    var possible_line_height = {};
                    if (!$this.is('textarea') && $this.css('height') != 'auto') {
                        possible_line_height = { lineHeight: $this.css('height'), whiteSpace: 'nowrap' };
                    }

                    var ol = $('<label />')
                      .text($this.attr('placeholder'))
                      .addClass(config.cls)
                      .css($.extend({
                          position: 'absolute',
                          display: 'inline',
                          float: 'none',
                          overflow: 'hidden',
                          textAlign: 'left',
                          color: config.color,
                          cursor: 'text',
                          paddingTop: $this.css('padding-top'),
                          paddingRight: $this.css('padding-right'),
                          paddingBottom: $this.css('padding-bottom'),
                          paddingLeft: $this.css('padding-left'),
                          fontSize: $this.css('font-size'),
                          fontFamily: $this.css('font-family'),
                          fontStyle: $this.css('font-style'),
                          fontWeight: $this.css('font-weight'),
                          textTransform: $this.css('text-transform'),
                          backgroundColor: 'transparent',
                          zIndex: 99
                      }, possible_line_height))
                      .css(calcPositionCss(this))
                      .attr('for', this.id)
                      .data('target', $this)
                      .click(function () {
                          $(this).data('target').focus();
                      })
                      .insertBefore(this);
                    $this
                      .data('placeholder', ol)
                      .focus(function () {
                          ol.hide();
                      }).blur(function () {
                          ol[$this.val().length ? 'hide' : 'show']();
                      }).triggerHandler('blur');
                    $(window).one("resize", function () { adjustToResizing(ol); });
                }
            });
        }
    });
})(afjQuery);

afjQuery(document).add(window).bind('ready load', function () {
    if (afjQuery.placeholder) {
        afjQuery.placeholder.shim();
    }
});



function initForm(formRoot, options, localization) {
    var $ = afjQuery;
    formRoot = $(formRoot);

    $('form').validate({
        errorElement: 'span',
        errorClass: 'help-inline inline-error',
        highlight: function (element, errorClass) {
            $(element).parents('.control-group:first').addClass('error');
        },
        unhighlight: function (element, errorClass) {
            $(element).parents('.control-group:first').removeClass('error');
        },
        errorPlacement: function (error, element) {
            var errPlace = element.parents('.control-group:first').find('.err-placeholder');
            if (errPlace.size()) {
                errPlace.append(error);
            } else {
                if (element.is(':checkbox') || element.is(':radio')) {
                    element.parent().append(error);
                } else {
                    error.insertAfter(element);
                }
            }
        }
    });

    // initialize password confirm
    $('[data-password-confirm]').each(function () {
        $(this).rules("add",{
            equalTo: '#' + $(this).attr('data-password-confirm'),
            messages: {
                equalTo: 'Password fields do not match.'
            }
        });
        //$('#' + $(this).attr('data-password-confirm')).rules("add", {
        //    equalTo: '#' + $(this).attr('id'),
        //    messages: {
        //        equalTo: 'Password fields do not match.'
        //    }
        //});
    });


    // load localized error messages
    for (var key in localization) {
        if (key.indexOf('validation.') == 0) {
            var relKey = key.substr('validation.'.length);
            $.validator.messages[relKey] = localization[key].indexOf('{0}') == -1 ? localization[key] : $.validator.format(localization[key]);
        }
    }

    //// load strings from the server
    //$.getJSON(formRoot.attr("")

    // fix for datepicker, onchange does not trigger validation, so call onkeyup manually
    $('form .datepicker').change(function () {
        $(this).keyup();
    });

    // initialize input masks
    formRoot.find('[data-mask]').each(function () {
        $(this).mask($(this).attr('data-mask'));
    });

    // initialize rich edits
    yepnope({
        test: formRoot.find(".richedit").size() > 0,
        yep: [
            options.appRoot + '/js/wysiwyg/jquery.wysiwyg2.min.js',
            options.appRoot + '/js/wysiwyg/jquery.wysiwyg.css',
        ],
        callback: function (url, result, key) {
            formRoot.find(".richedit").wysiwyg({
                autoGrow: false,
                maxHeight: 600,
                initialMinHeight: 50,
                initialContent: '',
                brIE: false,
                replaceDivWithP: true
            });

            // localize wysiwyg
            $('.wysiwyg [role="menuitem"]').each(function () {
                var l = localization['wysiwyg.' + $(this).attr('class')];
                l && $(this).attr('title', l);
            });
        }
    });

    // intialize date pickers
    formRoot.find(".datepicker").each(function () {
        var opts = {
            dateFormat: $(this).attr('data-dateformat'),
            changeMonth: $(this).attr('data-changemonth') == 'true',
            changeYear: $(this).attr('data-changeyear') == 'true'
        };

        if ($(this).attr('data-yearrange'))
            opts["yearRange"] = $(this).attr('data-yearrange');

        $(this).datepicker(opts);
        var theme = $(this).attr('data-theme');
        $('#ui-datepicker-div').each(function () {
            if ($(this).parent("." + theme).size() == 0)
                $(this).wrap('<div class="' + theme + '"></div>');
        });
    });

    //formRoot.find(".cancel").click(function () {
    //    formRoot.find('.submit').addClass('disabled');
    //    $(this).button('loading');
    //    window.location.reload();
    //});

    var regionCache = {};
    var populateRegions = function (regionRoot, data) {
        regionRoot.find('.region-loading, .region-textbox,.region-dropdown').hide();
        if (data.length == 0) {
            regionRoot.find('.region-textbox').show().change();
            regionRoot.find('label').attr('for', regionRoot.find('.region-textbox').attr('id'));
        } else {
            var dd = regionRoot.find('.region-dropdown').empty();
            regionRoot.find('label').attr('for', regionRoot.find('.region-dropdown').attr('id'));
            for (var i = 0; i < data.length; i++) {
                var key = dd.attr('data-mode') == 'code' ? data[i].key : data[i].value;
                dd.append('<option value="' + key + '" data-code="' + data[i].key + '">' + data[i].value + '</option>');
            }
            dd.show().change();
        }
    };

    formRoot.find('.country').change(function () {
        var regionRoot = $(this).parents('.country-root:first').next('.region-root');
        if (!regionRoot.size())
            return;

        var code = $(this).find(':selected').attr('data-code');
        if (regionCache[code]) {
            populateRegions(regionRoot, regionCache[code]);
        } else {
            regionRoot.find('.region-textbox,.region-dropdown').hide();
            regionRoot.find('.region-loading').show();
            $.getJSON(options.appRoot + '/GetDnnList.ashx', {
                listName: 'Region',
                parentKey: 'Country.' + code
            }, function (data) {
                if (data.error) {
                    alert(data.error); // ??
                }
                regionCache[code] = data;
                populateRegions(regionRoot, regionCache[code]);
            });
        }
    }).change();

    formRoot.find('.region-textbox,.region-dropdown').change(function () {
        var root = $(this).parents('.region-root:first');
        root.find('.region-value').val($(this).val());
    });

    // init file upload
    formRoot.find('.file-upload').each(function () {

        var $root = $(this).parents('.controls:first');

        $(this).fileupload({
            url: $(this).attr('data-uploadurl'),
            dataType: 'json',
            //autoUpload: false,
            add: function (e, data) {
                //if (data.autoUpload || (data.autoUpload !== false &&
                //        $(this).fileupload('option', 'autoUpload'))) {
                //    data.process().done(function () {
                //        data.submit();
                //    });
                //}
                this.upldata = data;
                $.each(data.files, function (index, file) {
                    //console.log(file);
                    $root.find('.files').empty().append($('<p/>').text(file.name));
                    $root.find('.files').show();
                    //$root.find('.relative-url').text(file.relativeurl);
                });
            },
            done: function (e, data) {
                $.each(data.result, function (index, file) {
                    $root.find('.files').empty().append($('<p/>').text(file.name));
                    $root.find('.relative-url').text(file.relativeurl);
                });
                $root.find('.progress').fadeOut('fast', function () {
                    $root.find('.files').show();
                });
                formRoot.toUpload--;
                if (formRoot.toUpload == 0)
                    submitData(formRoot.$btn);
            },
            progressall: function (e, data) {
                $root.find('.files').hide();
                $root.find('.progress').fadeIn('fast');
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $root.find('.progress .bar').css(
	                'width',
	                progress + '%'
	            );
            }
        });


        // hack for DNN 7 to leave our upload field alone
        var btn = $root.find('.fileinput-button');
        if (btn.find('.dnnInputFileWrapper').size() > 0) {
            btn.find('input').appendTo(btn);
            btn.find('.dnnInputFileWrapper').remove();
        } else {
            btn.find('input')[0].wrapper = 'hack';
        }

    })

    formRoot.find('input:text').keydown(function (evt) {
        if (evt.keyCode == 13) {
            formRoot.find('.submit').click();
        }
    });

    formRoot.on('click', ".form-button", function () {

        // reset
        formRoot.find(".server-error").html("").hide();

        var causesValidation = $(this).attr('data-validation') == 'on';

        if (causesValidation && !formRoot.find('input,textarea,select').valid()) {
            formRoot.find('.error:first').find('input,textarea,select').focus();
            return false;
        }

        // check if we need to start uploads
        formRoot.toUpload = 0;
        formRoot.$btn = $(this);
        formRoot.find('.file-upload').each(function () {

            $(this).fileupload("option", "formData", getFormData(formRoot));

            var data = this.upldata;
            if (data) {
                formRoot.toUpload++;
                data.process().done(function () {
                    data.submit();
                });
            }
        });
        
        var $btn = $(this);
        $btn.hasClass('loading') && $btn.button('loading');
        formRoot.find('.submit').not($btn).attr('disabled', 'disabled');
        formRoot.find('.submit-progress').stop(true, true).fadeIn();

        if (!formRoot.toUpload)
            submitData($btn);

        return false;
    });

    function getFormData(formRoot) {
        var data = {};
        formRoot.find('input').filter(':text,:password,:hidden').add(formRoot.find("select,textarea")).each(function () {
            if (!$(this).attr("name"))
                return;
            data[$(this).attr("name").replace(/dnn\d+/, "")] = $(this).val();
        });

        formRoot.find('.upload-root').each(function () {
            data[$(this).find('input').attr("name").replace(/dnn\d+/, "")] = $(this).find('.relative-url').text();
        });

        formRoot.find(":checkbox").each(function () {
            if (!$(this).attr("name"))
                return;
            var name = $(this).attr("name").replace(/dnn\d+/, "");
            if (name.indexOf('-') > 0) {
                // this is a checkbox list
                name = name.substr(0, name.indexOf('-'));
                if (!data[name])
                    data[name] = '';

                if ($(this).is(':checked'))
                    data[name] += (data[name].length ? ';' : '') + $(this).val();

            } else {
                data[name] = $(this).is(':checked') ? 'True' : 'False';
            }
        });
        formRoot.find(":radio:checked").each(function () {
            data[$(this).attr("name").replace(/dnn\d+/, "")] = $(this).val();
        });

        formRoot.find(".itemwithqty input").each(function () {
            data[$(this).attr("name").replace(/dnn\d+/, "")] = $('#' + $(this).attr("id") + 'Qty').val() + ' ' + $(this).val();
        });

        return data;
    }

    function submitData($btn) {

        if (formRoot[0].submitting)
            return;

        var data = getFormData(formRoot);

        //formRoot.find('.cancel').hide();
        formRoot[0].submitting = true;
        $.ajax({
            url: $btn.attr("data-submiturl"),
            type: "post",
            data: data,
            dataType: "json"
        }).done(function (data) {
            formRoot[0].submitting = false;
            parseFormResponse(data, {
                error: function (err) {
                    formRoot.find(".server-error").html(data.error).show();
                    $btn.button('reset');
                    formRoot.find('.submit').not($btn).removeAttr('disabled');
                    formRoot.find('.submit-progress').stop(true, true).fadeOut();
                },
                message: function (msg, type) {
                    if (!type || type == 'success')
                        formRoot.find(".c-form").slideUp();
                    console.log('now:' + msg);
                    formRoot.find(".submit-confirm").html(msg).show();
                    $btn.button('reset');
                    formRoot.find('.submit').not($btn).removeAttr('disabled');
                    formRoot.find('.submit-progress').stop(true, true).fadeOut();
                },
                appendHtml: function (appendHtml, appendTo) {
                    $(appendTo).append(appendHtml);
                    $btn.button('reset');
                    formRoot.find('.submit').not($btn).removeAttr('disabled');
                    formRoot.find('.submit-progress').stop(true, true).fadeOut();
                }
            });
        });

    };
}

function parseFormResponse(data, handlers) {

    var $ = afjQuery;

    // initialize with default handlers, unless provieded by caller
    handlers = $.extend({
        keepOnPage: function (url) {
            window.location.reload(true);
        },
        redirect: function (url) {
            window.location = url;
        },
        appendHtml: function (appendHtml, appendTo) {
            $(appendTo).append(appendHtml);
        },
        error: function (err) { },
        message: function (msg, type) { }
    }, handlers);

    // parse response and call handlers
    if (data.error) {
        handlers.error && handlers.error(data.error);
    } else if (data.messageHtml) {
        handlers.message && handlers.message(data.messageHtml, data.type);
    } else if (data.keepOnPage) {
        handlers.keepOnPage && handlers.keepOnPage(data.redirect);
    } else if (data.redirect) {
        handlers.redirect && handlers.redirect(data.redirect);
    } else if (data.appendHtml) {
        handlers.appendHtml && handlers.appendHtml(data.appendHtml, data.appendTo);
    }
}

