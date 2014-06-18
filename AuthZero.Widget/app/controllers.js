
app.controller('mainController', ['$scope', '$location', 'auth', 'AUTH_EVENTS', function ($scope, $location, auth, AUTH_EVENTS) {

    $scope.auth = auth;
    $scope.loggedIn = auth.isAuthenticated;

    $scope.$on(AUTH_EVENTS.loginSuccess, function () {
        $scope.loggedIn = true;
        $location.path('/shipments');
    });
    $scope.$on(AUTH_EVENTS.loginFailure, function () {
        console.log("There was an error");
    });

    $scope.login = function () {
        auth.signin({ popup: false });
    }

    $scope.logout = function () {
        auth.signout();
        $scope.loggedIn = false;
        $location.path('/');
    };

}]);

app.controller('shipmentsController', ['$scope', '$http', '$location', function ($scope, $http, $location) {

    //var serviceBase = "http://localhost:15482/";
    var serviceBase = "http://auth0api.azurewebsites.net/";
    $scope.shipments = [];

    init();

    function init() {
        getShipments();
    }

    function getShipments() {

        var shipmentsSuccess = function (response) {

            $scope.shipments = response.data;
        }

        var shipmentsFail = function (error) {

            if (error.status === 401) {
                $location.path('/');
            }
        }

        $http.get(serviceBase + 'api/shipments').then(shipmentsSuccess, shipmentsFail);

    }

}]);