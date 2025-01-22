import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';

import { registerLicense } from '@syncfusion/ej2-base';

// Registering Syncfusion license key
registerLicense('Ngo9BigBOggjHTQxAR8/V1NMaF5cXmBCf0x0THxbf1x1ZFFMZF1bRnRPMyBoS35Rc0ViW31edHBURGJeWEx+');

platformBrowserDynamic().bootstrapModule(AppModule)
.catch((err : any) =>
  console.error(err)
);
