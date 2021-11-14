import {
  createAsyncThunk,
  createEntityAdapter,
  createSlice,
} from '@reduxjs/toolkit'
import { RootState } from 'store/rootReducer'
import { AddGroupRequest, Group, RemoveGroupRequest } from '../../api/api.types'
import * as blogApi from '../../api/blogApi'
const GROUPS = 'groups'

export const groupsAdapter = createEntityAdapter<Group>({
  selectId: x => x.id,
})

export const getList = createAsyncThunk<
  // Return type of the payload creator
  Group[],
  // First argument to the payload creator
  void,
  unknown
>(`${GROUPS}/list`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.getGroupsList()
    return res
  } catch (err) {
    return rejectWithValue(err.response.data)
  }
})

export const postAdd = createAsyncThunk<
  // Return type of the payload creator
  Group,
  // First argument to the payload creator
  AddGroupRequest,
  unknown
>(`${GROUPS}/add`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.postAddGroup(params)
    return res
  } catch (err) {
    throw err.data
  }
})

export const remove = createAsyncThunk<
  // Return type of the payload creator
  number,
  // First argument to the payload creator
  RemoveGroupRequest,
  unknown
>(`${GROUPS}/remove`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.removeGroup(params)
    return params.groupId
  } catch (err) {
    throw err.data
  }
})

const groupsSlice = createSlice({
  name: 'groups',
  initialState: groupsAdapter.getInitialState(),
  reducers: {},
  extraReducers: builder => {
    builder
      .addCase(getList.fulfilled, (state, { payload }) => {
        groupsAdapter.setAll(state, payload)
      })
      .addCase(postAdd.fulfilled, (state, { payload }) => {
        groupsAdapter.addOne(state, payload)
      })
      .addCase(remove.fulfilled, (state, { payload }) => {
        groupsAdapter.removeOne(state, payload)
      })
  },
})

export const { actions } = groupsSlice

export default groupsSlice.reducer

export const stateSelector = (state: RootState) => state.groupsReducer

export const {
  selectAll,
  selectById,
  selectIds,
} = groupsAdapter.getSelectors<RootState>(state => state.groupsReducer)
