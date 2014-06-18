var app = angular.module('auth0-sample', ['auth0-redirect', 'ngRoute', 'authInterceptor']);

app.config(function (authProvider, $httpProvider, $routeProvider) {
    authProvider.init({
        domain: 'tjoudeh.auth0.com',
        clientID: '80YvW9Bsa5P67RnMZRJfZv8jEsDSerDW',
        callbackURL: location.href
    });

    $httpProvider.interceptors.push('authInterceptor');

    $routeProvider.when("/shipments", {
        controller: "shipmentsController",
        templateUrl: "/app/views/shipments.html"
    });

    $routeProvider.otherwise({ redirectTo: "/" });
});

