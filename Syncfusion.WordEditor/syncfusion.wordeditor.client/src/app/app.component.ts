import { HttpClient } from '@angular/common/http';
import { Component, ViewEncapsulation, ViewChild, OnInit } from '@angular/core';
import {
  ToolbarService,
  DocumentEditorContainerComponent,
} from '@syncfusion/ej2-angular-documenteditor';
import { TitleBar } from './title-bar';
// import { defaultDocument, WEB_API_ACTION } from './data';
import { isNullOrUndefined } from '@syncfusion/ej2-base';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  encapsulation: ViewEncapsulation.None,
  providers: [ToolbarService],
})

export class AppComponent implements OnInit {
  public forecasts: WeatherForecast[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    // this.getForecasts();
  }

  getForecasts() {
    this.http.get<WeatherForecast[]>('/weatherforecast').subscribe(
      (result) => {
        this.forecasts = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  title = 'syncfusion.wordeditor.client';

  public hostUrl: string =
    'https://services.syncfusion.com/angular/production/api/documenteditor/';
  @ViewChild('documenteditor_default')
  public container!: DocumentEditorContainerComponent;
  public culture: string = 'en-US';
  titleBar!: TitleBar | null;

  onCreate(): void {
    let titleBarElement: HTMLElement | null =
      document.getElementById('default_title_bar');
    this.titleBar = new TitleBar(
      titleBarElement!,
      this.container.documentEditor,
      true
    );
    // this.container.documentEditor.open(JSON.stringify(defaultDocument));
    this.container.documentEditor.documentName = 'Getting Started';
    this.titleBar.updateDocumentTitle();
  }

  onDocumentChange(): void {
    if (!isNullOrUndefined(this.titleBar)) {
      this.titleBar!.updateDocumentTitle();
    }
    this.container.documentEditor.focusIn();
  }
}
