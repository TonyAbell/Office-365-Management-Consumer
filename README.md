# Office 365 Management Consumer

## Steps Overview

1. Generate a Azure AD Application.
1. Office 365 Tenant Admin Consent.
1. Deploy the template.
1. Run 'step_1_create_subscription' Azure Function.
1. Run 'step_2_manually_query_content' Azure Functions.
1. Connect Power BI to Cosmos DB and build dashboards.

## Generate a Azure AD Application

Follow the instructions [here](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal#create-an-azure-active-directory-application) to create an Azure AD Application

You can use `http://localhost` as an Redirect Url

Take note of the following and save for later

- The Application Id is the Client Id
- The Key is the Client Secret

Set the AD Application Permissions

Navigate to the AD Application Permissions Pane

![](/readme-imgs/ad_permissions.png)

Add API the the AD Application

![](/readme-imgs/ad_permissions_add_api.png)

Select Management API

![](/readme-imgs/ad_permissions_select_managment_api.png)

Select Permissions 

![](/readme-imgs/ad_permissions_select_permissions.png)

### Get the Office 365 Tenant Id

Follow the instructions [here](https://support.office.com/en-us/article/Find-your-Office-365-tenant-ID-6891b561-a52d-4ade-9f39-b492285e2c9b) to get your Office 365 Tenant Id

## Office 365 Tenant Admin Consent

For more information review this [link](https://msdn.microsoft.com/office-365/get-started-with-office-365-management-apis#office-365-tenant-admin-consent)

Update '{your_client_id}' in this URL
`
https://login.windows.net/common/oauth2/authorize?response_type=code&resource=https%3A%2F%2Fmanage.office.com&client_id={your_client_id}&redirect_uri=http%3A%2F%2Flocalhost }
`

Have an Office 365 Admin Browse to the updated url and accept the permissions.

## Deploy Template

### Option 1

[![Deploy to Azure](http://azuredeploy.net/deploybutton.svg)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FTonyAbell%2FOffice-365-Management-Consumer%2Fmaster%2Fazuredeploy.json)

### Option 2

Run the following script.
```
az login
$rg="my-rg"
az group create --name $rg --location WestUS2
az group deployment validate  --resource-group $rg --template-uri 
"https://raw.githubusercontent.com/TonyAbell/Office-365-Management-Consumer/master/azuredeploy.json" --parameters appName={app name} --parameters ClientId={client id} --parameters ClientSecret={client secret} --parameters Tenantid={tenant id}
//remove source control deployment from project
az webapp deployment source delete -g $rg -n {generated app name}
```

## Run step_1_create_subscription

To edit the function you will need to remove the GitHub binding

Navigate to the deployments settings of the and disconnect the GitHub binding.

![](/readme-imgs/deploy_setting.png)

![](/readme-imgs/deploy_setting_disconnet.png)

Get the Web Hook Url

![](/readme-imgs/webhoot_url.png)

Set the Web Hook Url in the 'step_1_create_subscription' function.

![](/readme-imgs/set_webhook_url.png)

Save and run the function.

## Run step_2_manually_query_content

Run the step_2.. function

You will have to restart the Web App.

![](/readme-imgs/restart_web_app.png)

`
There is a bug in the template which requires a restart of the Web App at this point.  The process_content function fails to connect to the Document DB until data is created. Data is only created during the step 2 processes.
`

## Connect Power BI to Cosmos DB and build dashboards

Get Cosmos DB Read Only Key

![](/readme-imgs/get_db_keys.png)

Using Power BI to connect to Cosmos DB

![](/readme-imgs/pbi_cosmos_db.png)


### Links

- [Get started with Office 365 Management APIs](https://msdn.microsoft.com/office-365/get-started-with-office-365-management-apis)