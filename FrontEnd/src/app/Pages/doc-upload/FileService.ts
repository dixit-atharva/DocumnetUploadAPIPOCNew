import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FileService {

  constructor(private http: HttpClient) { }

  getFilePaths(folderPath: string): Observable<string[]> {
    const apiUrl = `http://localhost:5281/api/DocUpload/filecount?folderPath=${folderPath}`;
    return this.http.get<string[]>(apiUrl);
  }
}
