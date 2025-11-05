import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

/**
 * Application bootstrap entry point
 * Initializes the Angular application with configuration
 */
bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
