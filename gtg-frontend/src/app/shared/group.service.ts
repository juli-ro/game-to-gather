import {Injectable, signal, WritableSignal} from '@angular/core';
import {ApiDataService} from './api-data.service';
import {IGroup} from '../models/group';
import {HttpClient} from '@angular/common/http';
import {Router} from '@angular/router';
import {lastValueFrom} from 'rxjs';
import {FeedbackService} from './feedback.service';
import {Location} from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class GroupService extends ApiDataService<IGroup> {

  protected override signalList: WritableSignal<IGroup[]> = signal<IGroup[]>([])
  protected override signalItem: WritableSignal<IGroup | null> = signal<IGroup | null>(null)
  override publicSignalList = this.signalList.asReadonly()
  override publicSignalItem = this.signalItem.asReadonly()

  constructor(override httpClient: HttpClient,
              override router: Router,
              override location: Location,
              override feedbackService: FeedbackService) {
    super(httpClient, router, location, feedbackService)
  }

  override getResourceUrl(): string {
    return 'group';
  }

  async getUserGroupList(): Promise<void> {
    try {
      const data = await lastValueFrom(
        this.httpClient.get<IGroup[]>(`${this.APIUrl}/UserGroup`, await this.getHttpOptions())
      )
      this.signalList.set(data)
    } catch (error) {
      await this.handleError(error)
    }
  }

  async createNewGroup(){
    try {
      const data = await lastValueFrom(
        this.httpClient.post<string>(`${this.APIUrl}/UserGroup`, {}, await this.getHttpOptions())
      )
      if(data){
        return data;
      }
      else return ""
    } catch (error) {
      await this.handleError(error)
      //Todo: handle this in handleError?
      this.feedbackService.openStandardSnackBarTimed("Error: Group could not be added")
      return null
    }
  }

}
