const securityPoints = {
    configuration: {
        add: ['ConsumerAuthorizationServices.Manage.Administrators', 'ConsumerAuthorizationServices.Manage.Users'],
        edit: ['ConsumerAuthorizationServices.Manage.Administrators', 'ConsumerAuthorizationServices.Manage.Users'],
        delete: ['ConsumerAuthorizationServices.Manage.Administrators', 'ConsumerAuthorizationServices.Manage.Users'],
        view: ['ConsumerAuthorizationServices.Manage.Administrators', 'ConsumerAuthorizationServices.Manage.Users'],
    },
    testUserAction: {
        add: ['ConsumerAuthorizationServices.Manage.Administrators', 'ConsumerAuthorizationServices.Manage.Users'],
        edit: ['ConsumerAuthorizationServices.Manage.Administrators', 'ConsumerAuthorizationServices.Manage.Users'],
        delete: ['ConsumerAuthorizationServices.Manage.Administrators', 'ConsumerAuthorizationServices.Manage.Users'],
        view: ['ConsumerAuthorizationServices.Manage.Administrators', 'ConsumerAuthorizationServices.Manage.Users'],
    }
}

export default securityPoints