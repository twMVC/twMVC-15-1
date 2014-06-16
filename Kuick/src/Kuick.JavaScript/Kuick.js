// Kuick

// Extensions
(function () {
	// String.trim
	if (typeof String.prototype.trim !== 'function') {
		String.prototype.trim = function () { return this.replace(/^\s+|\s+$/g, ''); };
	}
	// String.startsWith
	if (typeof String.prototype.startsWith != 'function') {
		String.prototype.startsWith = function (str) { return this.indexOf(str) == 0; };
	}
	// Array.indexOf
	if (typeof Array.prototype.indexOf != 'function') {
		Array.prototype.indexOf = function (searchElement) {
			"use strict";
			if (this == null) { throw new TypeError(); }
			var t = Object(this);
			var len = t.length >>> 0;
			if (len === 0) { return -1; }
			var n = 0;
			if (arguments.length > 0) {
				n = Number(arguments[1]);
				if (n != n) {
					n = 0;
				} else if (n != 0 && n != Infinity && n != -Infinity) {
					n = (n > 0 || -1) * Math.floor(Math.abs(n));
				}
			}
			if (n >= len) { return -1; }
			var k = n >= 0 ? n : Math.max(len - Math.abs(n), 0);
			for (; k < len; k++) {
				if (k in t && t[k] === searchElement) { return k; }
			}
			return -1;
		};
	}
	// Array.forEach
	if (!('forEach' in Array.prototype)) {
		Array.prototype.forEach = function (action, that) {
			"use strict";
			for (var i = 0, n = this.length; i < n; i++) {
				if (i in this) { action.call(that, this[i], i, this); }
			}
		};
	}
	// String.replaceAll
	if (!('replaceAll' in String.prototype)) {
		String.prototype.replaceAll = function (replaceFrom, replaceTo) {
			return this.split(replaceFrom).join(replaceTo);
		}
	}
})();


// StringBuilder
function StringBuilder() { this.tmp = new Array(); }
StringBuilder.prototype.Append = function (value) { this.tmp.push(value); return this; }
StringBuilder.prototype.Clear = function () { tmp.length = 1; }
StringBuilder.prototype.toString = function () { return this.tmp.join(''); }


// Mobile client detect
window.isMobileClient = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
window.clickEventName = window.isMobileClient ? 'touchstart' : 'click';

// Asynchronous
window.Asynchronous = new Array();
window.AsyncExecute = function (fn) {
	window.Asynchronous.push(fn);
};


// Validation
window.Validations = {
	errors: new Array(),
	clickToSubmit: function ($Object, operation) {
		operation = typeof (operation !== 'undefined') ? operation : {};
		$Object.click(function () {
			var $form = $(this).closest("form").eq(0);
			if ($form) {
				window.inputSubmit($form, opt);
			}
		});
	},
};
