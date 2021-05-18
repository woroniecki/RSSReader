import { API_URL } from './../utils/envs'
import axios, { AxiosRequestConfig } from 'axios'

const axiosRequestConfig: AxiosRequestConfig = {
  baseURL: API_URL(),
  headers: { Version: '1.0', 'Content-Type': 'application/json' },
}

const instance = axios.create(axiosRequestConfig)
export default instance
