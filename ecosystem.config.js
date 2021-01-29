/**
 * An environment file for pm2.
 * See: https://pm2.keymetrics.io/docs/usage/quick-start/
 */

module.exports = {
	apps: [
    	{
			name: "wilding-pines-api-server",
			script: "cd scripts/start-up && bash start-api-server.sh",
			env: {
				"ASPNETCORE_URLS": "https://0.0.0.0:5001;http://0.0.0.0:5000",
			}
		},
    	{
			name: "wilding-pines-spray-module",
			script: "cd scripts/start-up && bash start-spray-module.sh",
    	},
    	{
    			name: "wilding-pines-aimms-interface",
    			script: "cd scripts/start-up && bash start-aimms-module.sh"
    	}
  	],
};
