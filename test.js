import billing from './billing'
import billingService from 'services/billing'
import {queryBilling, updatePrimaryBillingContact} from 'services/billing'
import {call, put, select} from 'redux-saga/effects';
describe('billing', ()=>{
    describe('namespace',()=>{
        it('Should have correct name', () => {
          
            // spyOn(billingService, 'queryBilling').and.callFake(()=> {
            //     return Promise.resolve()
            // })
            // expect(billing.namespace).toEqual('billing')
            
            const action = () => { }
            const gen = billing.effects.query(action,{call,put})
            const returnedValue = {"done": false,"value":call(queryBilling)}
            expect(gen.next()).toEqual(returnedValue)
        })
    })
})
