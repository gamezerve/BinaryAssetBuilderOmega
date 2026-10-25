movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FEC8F2~1\\FEG_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &constants 'feg_m_messageBox::onLoad()', 'onEnterFrame', 'frameDelayedInit', 'bind0', 'm_refGlobalEventHandler', 'gEH', 'button1Clicked', 'messageBoxMC', 'button1', 'setOnMouseUpFunction', 'button2Clicked', 'button2', 'button3Clicked', 'button3', 'button4Clicked', 'button4', 'localMessageHandler', 'gMH', 'addMessageHandler', 'enable', 'noIntro', 'disable', 'onHide', 'sendResponseToEngine', 'feg_m_messageBox::onUnload()', 'removeMessageHandler', 'feg_m_messageBox', 'feg_m_messageBox::localMessageHandler() messageCode = \'', '\'', 'MSGCODE', 'FE_MESSAGE_SHOW', 'FE_MESSAGE_HIDE', 'loadData', 'feg_m_messageBox::loadData()', 'Object', 'QueryGameEngine?MESSAGE_INFO', 'Array', 'MESSAGE_INFO_OPTION1', 'length', '$', 'push', 'MESSAGE_INFO_OPTION2', 'MESSAGE_INFO_OPTION3', 'MESSAGE_INFO_OPTION4', 'MESSAGE_INFO_TEXT', 'MESSAGE_INFO_TITLE', 'MESSAGE_INFO_ID', 'showMessageBox', 'feg_m_messageBox::showMessageBox()', 'm_messageID', 'messageID', 'm_messageCallback', 'messageCallback', 'getCurrentLocalEventHandler', 'setLocalEventHandler', 'm_eventHandlerIndex', 'm_eventHandlerPriority', 'localEventHandler', 'addPriorityLocalEventHandler', 'this', 'messageTF', 'autoSize', 'titleTF', 'text', 'messageTitle', '&Outline', 'messageText', 'messageHeight', 'textHeight', 'V_PADDING', 'messageBottom', '_y', 'bodyHeight', 'm_handleEvents', 'messageOptions', 'show', 'buttonWidth', 'H_PADDING', 'getWidth', 'buttonHeight', 'getHeight', 'buttonBarWidth', 'buttonBarX', 'panelMiddleMC', '_width', '_x', 'btnMC', 'index', 'this.messageBoxMC.button', 'setText', '_height', 'panelBottomMC', 'OVERLAP_OFFSET', 'onShow', 'm_FM', 'handleDefaultInputs', '%Message_Response?MessageID=', '|ResponseID=', 'feg_m_messageBox::sendResponseToEngine() commandStr == ', 'FSCommand:CallGameFunction', 'feg_m_messageBox::onShow()', 'shadeMC', '_visible', 'm_playAudio', 'feg_m_messageBox::onHide()', '', 'hide', 'removeLocalEventHandler', 'Gui_GlobalShell_LoadingLights', 'playSound', 'feg_m_messageBox loading now', 'onLoad', 'feg_m_messageBox loaded'  
    &function2 onLoad () (r:1='this', r:2='_global')
      &pushsdb 0							//'feg_m_messageBox::onLoad()'
      &trace
      &pushregister 1
      &pushsdb 1							//'onEnterFrame'
      &pushsdbgv 2							//'frameDelayedInit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 3							//'bind0'
      &callMethod
      &setMember
    &end // of function onLoad

    &function2 frameDelayedInit () (r:1='this', r:2='_root')
      &pushregister 1
      &pushsdb 1							//'onEnterFrame'
      &pushnull
      &setMember
      &pushsdb 4							//'m_refGlobalEventHandler'
      &pushregister 2
      &pushsdbgm 5							//'gEH'
      &setVariable
      &pushregister 1
      &pushsdbgm 6							//'button1Clicked'
      &pushone
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &dcallmp 9							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 10							//'button2Clicked'
      &pushone
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 11							//'button2'
      &dcallmp 9							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 12							//'button3Clicked'
      &pushone
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 13							//'button3'
      &dcallmp 9							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 14							//'button4Clicked'
      &pushone
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 15							//'button4'
      &dcallmp 9							// setOnMouseUpFunction()
      &pushsdbgv 16							//'localMessageHandler'
      &pushone
      &pushsdbgv 17							//'gMH'
      &dcallmp 18							// addMessageHandler()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &dcallmp 19							// enable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 11							//'button2'
      &dcallmp 19							// enable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 13							//'button3'
      &dcallmp 19							// enable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 15							//'button4'
      &dcallmp 19							// enable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &dcallmp 20							// noIntro()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 11							//'button2'
      &dcallmp 20							// noIntro()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 13							//'button3'
      &dcallmp 20							// noIntro()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 15							//'button4'
      &dcallmp 20							// noIntro()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &dcallmp 21							// disable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 11							//'button2'
      &dcallmp 21							// disable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 13							//'button3'
      &dcallmp 21							// disable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 15							//'button4'
      &dcallmp 21							// disable()
      &pushzero
      &dcallfp 22							// onHide()
    &end // of function frameDelayedInit

    &function button1Clicked (    )    
      &pushzero
      &dcallfp 22							// onHide()
      &pushzero
      &pushone
      &dcallfp 23							// sendResponseToEngine()
    &end // of function button1Clicked

    &function button2Clicked (    )    
      &pushzero
      &dcallfp 22							// onHide()
      &pushone
      &pushone
      &dcallfp 23							// sendResponseToEngine()
    &end // of function button2Clicked

    &function button3Clicked (    )    
      &pushzero
      &dcallfp 22							// onHide()
      &pushbyte 2
      &pushone
      &dcallfp 23							// sendResponseToEngine()
    &end // of function button3Clicked

    &function button4Clicked (    )    
      &pushzero
      &dcallfp 22							// onHide()
      &pushbyte 3
      &pushone
      &dcallfp 23							// sendResponseToEngine()
    &end // of function button4Clicked

    &function2 onUnload () (r:1='_global')
      &pushsdb 24							//'feg_m_messageBox::onUnload()'
      &trace
      &pushsdbgv 16							//'localMessageHandler'
      &pushone
      &pushsdbgv 17							//'gMH'
      &dcallmp 25							// removeMessageHandler()
      &pushregister 1
      &pushsdb 26							//'feg_m_messageBox'
      &delete
      &pop
    &end // of function onUnload

    &function2 localMessageHandler (r:3='messageCode') (r:1='_global')
      &pushsdb 27							//'feg_m_messageBox::localMessageHandler() messageCode = \''
      &pushregister 3
      &add
      &pushsdb 28							//'\''
      &add
      &trace
      &pushfalse
      &setRegister r:2
      &pop
      &pushregister 3
      &setRegister r:0
      &pushregister 1
      &pushsdbgm 29							//'MSGCODE'
      &pushsdbgm 30							//'FE_MESSAGE_SHOW'
      &strictEquals
      &jnz label1      
      &pushregister 0
      &pushregister 1
      &pushsdbgm 29							//'MSGCODE'
      &pushsdbgm 31							//'FE_MESSAGE_HIDE'
      &strictEquals
      &jnz label2      
      &jmp label3      
     label1:
      &pushzero
      &dcallfp 32							// loadData()
      &pushtrue
      &setRegister r:2
      &pop
      &jmp label4      
     label2:
      &pushzero
      &dcallfp 22							// onHide()
      &pushtrue
      &setRegister r:2
      &pop
      &jmp label4      
     label3:
      &jmp label4      
     label4:
      &pushregister 2
      &return
    &end // of function localMessageHandler

    &function2 loadData () ()
      &pushsdb 33							//'feg_m_messageBox::loadData()'
      &trace
      &pushzero
      &pushsdb 34							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 35							//'QueryGameEngine?MESSAGE_INFO'
      &pushregister 1
      &loadVariables
      &pushzero
      &pushsdb 36							//'Array'
      &new
      &setRegister r:2
      &pop
      &pushregister 1
      &pushsdbgm 37							//'MESSAGE_INFO_OPTION1'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label5      
      &pop
      &pushregister 1
      &pushsdbgm 37							//'MESSAGE_INFO_OPTION1'
      &pushsdbgm 38							//'length'
      &pushzero
      &greaterThan
     label5:
      &not
      &jnz label6      
      &pushsdb 39							//'$'
      &pushregister 1
      &pushsdbgm 37							//'MESSAGE_INFO_OPTION1'
      &add
      &pushone
      &pushregister 2
      &dcallmp 40							// push()
     label6:
      &pushregister 1
      &pushsdbgm 41							//'MESSAGE_INFO_OPTION2'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label7      
      &pop
      &pushregister 1
      &pushsdbgm 41							//'MESSAGE_INFO_OPTION2'
      &pushsdbgm 38							//'length'
      &pushzero
      &greaterThan
     label7:
      &not
      &jnz label8      
      &pushsdb 39							//'$'
      &pushregister 1
      &pushsdbgm 41							//'MESSAGE_INFO_OPTION2'
      &add
      &pushone
      &pushregister 2
      &dcallmp 40							// push()
     label8:
      &pushregister 1
      &pushsdbgm 42							//'MESSAGE_INFO_OPTION3'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label9      
      &pop
      &pushregister 1
      &pushsdbgm 42							//'MESSAGE_INFO_OPTION3'
      &pushsdbgm 38							//'length'
      &pushzero
      &greaterThan
     label9:
      &not
      &jnz label10      
      &pushsdb 39							//'$'
      &pushregister 1
      &pushsdbgm 42							//'MESSAGE_INFO_OPTION3'
      &add
      &pushone
      &pushregister 2
      &dcallmp 40							// push()
     label10:
      &pushregister 1
      &pushsdbgm 43							//'MESSAGE_INFO_OPTION4'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label11      
      &pop
      &pushregister 1
      &pushsdbgm 43							//'MESSAGE_INFO_OPTION4'
      &pushsdbgm 38							//'length'
      &pushzero
      &greaterThan
     label11:
      &not
      &jnz label12      
      &pushsdb 39							//'$'
      &pushregister 1
      &pushsdbgm 43							//'MESSAGE_INFO_OPTION4'
      &add
      &pushone
      &pushregister 2
      &dcallmp 40							// push()
     label12:
      &pushsdbgv 23							//'sendResponseToEngine'
      &pushregister 2
      &pushsdb 39							//'$'
      &pushregister 1
      &pushsdbgm 44							//'MESSAGE_INFO_TEXT'
      &add
      &pushsdb 39							//'$'
      &pushregister 1
      &pushsdbgm 45							//'MESSAGE_INFO_TITLE'
      &add
      &pushregister 1
      &pushsdbgm 46							//'MESSAGE_INFO_ID'
      &pushbyte 5
      &dcallfp 47							// showMessageBox()
    &end // of function loadData

    &function showMessageBox ('messageID', 'messageTitle', 'messageText', 'messageOptions', 'messageCallback'    )    
      &pushsdb 48							//'feg_m_messageBox::showMessageBox()'
      &trace
      &pushsdb 49							//'m_messageID'
      &pushsdbgv 50							//'messageID'
      &setVariable
      &pushsdb 51							//'m_messageCallback'
      &pushsdbgv 52							//'messageCallback'
      &setVariable
      &pushzero
      &pushsdbgv 4							//'m_refGlobalEventHandler'
      &pushsdb 53							//'getCurrentLocalEventHandler'
      &callMethod
      &pushundef
      &equals
      &not
      &jnz label13      
      &pushnull
      &pushone
      &pushsdbgv 4							//'m_refGlobalEventHandler'
      &dcallmp 54							// setLocalEventHandler()
     label13:
      &pushsdb 55							//'m_eventHandlerIndex'
      &pushsdbgv 56							//'m_eventHandlerPriority'
      &pushsdbgv 57							//'localEventHandler'
      &pushbyte 2
      &pushsdbgv 4							//'m_refGlobalEventHandler'
      &dcallmsv 58							// addPriorityLocalEventHandler()
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 60							//'messageTF'
      &pushsdb 61							//'autoSize'
      &pushtrue
      &setMember
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 62							//'titleTF'
      &pushsdb 63							//'text'
      &pushsdbgv 64							//'messageTitle'
      &pushsdb 65							//'&Outline'
      &add
      &setMember
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 60							//'messageTF'
      &pushsdb 63							//'text'
      &pushsdbgv 66							//'messageText'
      &pushsdb 65							//'&Outline'
      &add
      &setMember
      &pushsdb 67							//'messageHeight'
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 60							//'messageTF'
      &pushsdbgm 68							//'textHeight'
      &pushsdbgv 69							//'V_PADDING'
      &add
      &setVariable
      &pushsdb 70							//'messageBottom'
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 60							//'messageTF'
      &pushsdbgm 71							//'_y'
      &pushsdbgv 67							//'messageHeight'
      &add
      &varEquals
      &pushsdb 72							//'bodyHeight'
      &pushbyte 2
      &pushsdbgv 69							//'V_PADDING'
      &multiply
      &pushsdbgv 67							//'messageHeight'
      &add
      &varEquals
      &pushsdb 73							//'m_handleEvents'
      &pushfalse
      &setVariable
      &pushsdbgv 74							//'messageOptions'
      &pushsdbgm 38							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label16      
      &pushsdb 73							//'m_handleEvents'
      &pushtrue
      &setVariable
      &pushzero
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &dcallmp 75							// show()
      &pushsdb 76							//'buttonWidth'
      &pushbyte 2
      &pushsdbgv 77							//'H_PADDING'
      &multiply
      &pushzero
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &pushsdb 78							//'getWidth'
      &callMethod
      &add
      &varEquals
      &pushsdb 79							//'buttonHeight'
      &pushzero
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &pushsdb 80							//'getHeight'
      &callMethod
      &varEquals
      &pushsdb 81							//'buttonBarWidth'
      &pushsdbgv 74							//'messageOptions'
      &pushsdbgm 38							//'length'
      &pushsdbgv 76							//'buttonWidth'
      &multiply
      &varEquals
      &pushsdb 82							//'buttonBarX'
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 83							//'panelMiddleMC'
      &pushsdbgm 84							//'_width'
      &pushsdbgv 81							//'buttonBarWidth'
      &subtract
      &pushbyte 2
      &divide
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 83							//'panelMiddleMC'
      &pushsdbgm 85							//'_x'
      &add
      &pushsdbgv 77							//'H_PADDING'
      &add
      &varEquals
      &pushsdb 86							//'btnMC'
      &var
      &pushsdb 87							//'index'
      &pushzero
      &varEquals
     label14:
      &pushsdbgv 87							//'index'
      &pushsdbgv 74							//'messageOptions'
      &pushsdbgm 38							//'length'
      &lessThan
      &not
      &jnz label15      
      &pushsdb 86							//'btnMC'
      &pushsdb 88							//'this.messageBoxMC.button'
      &pushsdbgv 87							//'index'
      &pushone
      &add
      &add
      &getVariable
      &setVariable
      &pushsdbgv 86							//'btnMC'
      &pushsdb 71							//'_y'
      &pushsdbgv 70							//'messageBottom'
      &setMember
      &pushsdbgv 86							//'btnMC'
      &pushsdb 85							//'_x'
      &pushsdbgv 82							//'buttonBarX'
      &pushsdbgv 87							//'index'
      &pushsdbgv 76							//'buttonWidth'
      &multiply
      &add
      &setMember
      &pushsdbgv 74							//'messageOptions'
      &pushsdbgv 87							//'index'
      &getMember
      &pushone
      &pushsdbgv 86							//'btnMC'
      &dcallmp 89							// setText()
      &pushzero
      &pushsdbgv 86							//'btnMC'
      &dcallmp 75							// show()
      &pushzero
      &pushsdbgv 86							//'btnMC'
      &dcallmp 19							// enable()
      &pushsdb 87							//'index'
      &pushsdbgv 87							//'index'
      &increment
      &setVariable
      &jmp label14      
     label15:
      &pushsdb 72							//'bodyHeight'
      &pushsdbgv 72							//'bodyHeight'
      &pushsdbgv 79							//'buttonHeight'
      &add
      &setVariable
     label16:
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 83							//'panelMiddleMC'
      &pushsdb 90							//'_height'
      &pushsdbgv 72							//'bodyHeight'
      &setMember
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 91							//'panelBottomMC'
      &pushsdb 71							//'_y'
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 83							//'panelMiddleMC'
      &pushsdbgm 71							//'_y'
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 83							//'panelMiddleMC'
      &pushsdbgm 90							//'_height'
      &add
      &pushsdbgv 92							//'OVERLAP_OFFSET'
      &subtract
      &setMember
      &pushzero
      &dcallfp 93							// onShow()
    &end // of function showMessageBox

    &function2 localEventHandler (r:1='keyCode', 'controller') ()
      &pushregister 1
      &pushone
      &pushsdbgv 94							//'m_FM'
      &dcallmp 95							// handleDefaultInputs()
    &end // of function localEventHandler

    &function2 sendResponseToEngine (r:2='index') ()
      &pushsdb 96							//'%Message_Response?MessageID='
      &pushsdbgv 49							//'m_messageID'
      &add
      &pushsdb 97							//'|ResponseID='
      &add
      &pushregister 2
      &add
      &setRegister r:1
      &pop
      &pushsdb 98							//'feg_m_messageBox::sendResponseToEngine() commandStr == '
      &pushregister 1
      &add
      &trace
      &pushsdb 99							//'FSCommand:CallGameFunction'
      &pushregister 1
      &getURL2
    &end // of function sendResponseToEngine

    &function2 onShow () (r:1='this')
      &pushsdb 100							//'feg_m_messageBox::onShow()'
      &trace
      &pushregister 1
      &pushsdbgm 101							//'shadeMC'
      &pushsdb 102							//'_visible'
      &pushtrue
      &setMember
      &pushshort 360
      &setRegister r:2
      &pop
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdb 71							//'_y'
      &pushregister 2
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 90							//'_height'
      &pushbyte 2
      &divide
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdb 102							//'_visible'
      &pushtrue
      &setMember
      &pushsdb 103							//'m_playAudio'
      &pushtrue
      &setVariable
    &end // of function onShow

    &function2 onHide () (r:1='this')
      &pushsdb 104							//'feg_m_messageBox::onHide()'
      &trace
      &pushsdb 103							//'m_playAudio'
      &pushfalse
      &setVariable
      &pushregister 1
      &pushsdbgm 101							//'shadeMC'
      &pushsdb 102							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdb 102							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 62							//'titleTF'
      &pushsdb 63							//'text'
      &pushsdb 105							//''
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 60							//'messageTF'
      &pushsdb 63							//'text'
      &pushsdb 105							//''
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &pushsdb 71							//'_y'
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 62							//'titleTF'
      &pushsdbgm 71							//'_y'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &dcallmp 106							// hide()
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 11							//'button2'
      &pushsdb 71							//'_y'
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 62							//'titleTF'
      &pushsdbgm 71							//'_y'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 11							//'button2'
      &dcallmp 106							// hide()
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 13							//'button3'
      &pushsdb 71							//'_y'
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 62							//'titleTF'
      &pushsdbgm 71							//'_y'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 13							//'button3'
      &dcallmp 106							// hide()
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 15							//'button4'
      &pushsdb 71							//'_y'
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 62							//'titleTF'
      &pushsdbgm 71							//'_y'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 15							//'button4'
      &dcallmp 106							// hide()
      &pushsdbgv 55							//'m_eventHandlerIndex'
      &pushbyte -1
      &equals
      &not
      &not
      &jnz label17      
      &pushsdbgv 55							//'m_eventHandlerIndex'
      &pushone
      &pushsdbgv 4							//'m_refGlobalEventHandler'
      &dcallmp 107							// removeLocalEventHandler()
     label17:
      &pushsdb 55							//'m_eventHandlerIndex'
      &pushbyte -1
      &setVariable
      &pushsdb 73							//'m_handleEvents'
      &pushfalse
      &setVariable
    &end // of function onHide

    &function2 playSoundFX () (r:1='_global')
      &pushsdbgv 103							//'m_playAudio'
      &not
      &jnz label18      
      &pushsdb 108							//'Gui_GlobalShell_LoadingLights'
      &pushone
      &pushregister 1
      &dcallmp 109							// playSound()
     label18:
    &end // of function playSoundFX

    &pushsdb 110							//'feg_m_messageBox loading now'
    &trace
    &pushsdb 77							//'H_PADDING'
    &pushbyte 5
    &varEquals
    &pushsdb 69							//'V_PADDING'
    &pushbyte 20
    &varEquals
    &pushsdb 49							//'m_messageID'
    &var
    &pushsdb 51							//'m_messageCallback'
    &var
    &pushsdb 103							//'m_playAudio'
    &pushfalse
    &varEquals
    &pushsdb 73							//'m_handleEvents'
    &pushfalse
    &varEquals
    &pushsdb 55							//'m_eventHandlerIndex'
    &pushbyte -1
    &varEquals
    &pushsdb 56							//'m_eventHandlerPriority'
    &pushone
    &varEquals
    &pushsdb 4							//'m_refGlobalEventHandler'
    &var
    &pushsdb 92							//'OVERLAP_OFFSET'
    &pushbyte 8
    &varEquals
    &pushzero
    &dcallfp 111							// onLoad()
    &pushsdb 112							//'feg_m_messageBox loaded'
    &trace
    &stop
  &end // of frame 0

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4

  &defineMovieClip 12 // total frames: 120
  &end // of defineMovieClip 12

  &defineMovieClip 15 // total frames: 1
  &end // of defineMovieClip 15
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 21
  &end // of importAssets

  &defineMovieClip 22 // total frames: 1

    &frame 0
      &pushs 'feg_m_MesaageBox start up'
      &trace
      &stop
    &end // of frame 0

    &placeMovieClip 21 &as 'button1'
    
      &onClipEvent &construct
        &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'buttonContentSymbol'  
        &pushsdb 0							//'m_width'
        &pushshort 200
        &setVariable
        &pushsdb 1							//'m_textAlign'
        &pushfalse
        &setVariable
        &pushsdb 2							//'m_labelPosition'
        &pushsdb 3							//'right align'
        &setVariable
        &pushsdb 4							//'m_label_x'
        &pushzerosv
        &pushsdb 5							//'m_label_y'
        &pushzerosv
        &pushsdb 6							//'m_focusDirs'
        &pushsdb 7							//'Up/Down'
        &setVariable
        &pushsdb 8							//'m_iconType'
        &pushsdb 9							//'default'
        &setVariable
        &pushsdb 10							//'m_extLabel'
        &pushsdb 11							//''
        &setVariable
        &pushsdb 12							//'m_nTruncateType'
        &pushzerosv
        &pushsdb 13							//'m_label'
        &pushsdb 11							//''
        &setVariable
        &pushsdb 14							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 15							//'m_refFM'
        &pushsdb 16							//'_root.gFM'
        &setVariable
        &pushsdb 17							//'m_initiallySelected'
        &pushfalse
        &setVariable
        &pushsdb 18							//'m_tabIndex'
        &pushzerosv
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_contentSymbol'
        &pushsdb 21							//'buttonContentSymbol'
        &setVariable
      &end
    &end // of placeMovieClip 21

    &placeMovieClip 21 &as 'button2'
    
      &onClipEvent &construct
        &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'buttonContentSymbol'  
        &pushsdb 0							//'m_width'
        &pushshort 200
        &setVariable
        &pushsdb 1							//'m_textAlign'
        &pushfalse
        &setVariable
        &pushsdb 2							//'m_labelPosition'
        &pushsdb 3							//'right align'
        &setVariable
        &pushsdb 4							//'m_label_x'
        &pushzerosv
        &pushsdb 5							//'m_label_y'
        &pushzerosv
        &pushsdb 6							//'m_focusDirs'
        &pushsdb 7							//'Up/Down'
        &setVariable
        &pushsdb 8							//'m_iconType'
        &pushsdb 9							//'default'
        &setVariable
        &pushsdb 10							//'m_extLabel'
        &pushsdb 11							//''
        &setVariable
        &pushsdb 12							//'m_nTruncateType'
        &pushzerosv
        &pushsdb 13							//'m_label'
        &pushsdb 11							//''
        &setVariable
        &pushsdb 14							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 15							//'m_refFM'
        &pushsdb 16							//'_root.gFM'
        &setVariable
        &pushsdb 17							//'m_initiallySelected'
        &pushfalse
        &setVariable
        &pushsdb 18							//'m_tabIndex'
        &pushone
        &setVariable
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_contentSymbol'
        &pushsdb 21							//'buttonContentSymbol'
        &setVariable
      &end
    &end // of placeMovieClip 21

    &placeMovieClip 21 &as 'button3'
    
      &onClipEvent &construct
        &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'buttonContentSymbol'  
        &pushsdb 0							//'m_width'
        &pushshort 200
        &setVariable
        &pushsdb 1							//'m_textAlign'
        &pushfalse
        &setVariable
        &pushsdb 2							//'m_labelPosition'
        &pushsdb 3							//'right align'
        &setVariable
        &pushsdb 4							//'m_label_x'
        &pushzerosv
        &pushsdb 5							//'m_label_y'
        &pushzerosv
        &pushsdb 6							//'m_focusDirs'
        &pushsdb 7							//'Up/Down'
        &setVariable
        &pushsdb 8							//'m_iconType'
        &pushsdb 9							//'default'
        &setVariable
        &pushsdb 10							//'m_extLabel'
        &pushsdb 11							//''
        &setVariable
        &pushsdb 12							//'m_nTruncateType'
        &pushzerosv
        &pushsdb 13							//'m_label'
        &pushsdb 11							//''
        &setVariable
        &pushsdb 14							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 15							//'m_refFM'
        &pushsdb 16							//'_root.gFM'
        &setVariable
        &pushsdb 17							//'m_initiallySelected'
        &pushfalse
        &setVariable
        &pushsdb 18							//'m_tabIndex'
        &pushbyte 2
        &setVariable
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_contentSymbol'
        &pushsdb 21							//'buttonContentSymbol'
        &setVariable
      &end
    &end // of placeMovieClip 21

    &placeMovieClip 21 &as 'button4'
    
      &onClipEvent &construct
        &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'buttonContentSymbol'  
        &pushsdb 0							//'m_width'
        &pushshort 200
        &setVariable
        &pushsdb 1							//'m_textAlign'
        &pushfalse
        &setVariable
        &pushsdb 2							//'m_labelPosition'
        &pushsdb 3							//'right align'
        &setVariable
        &pushsdb 4							//'m_label_x'
        &pushzerosv
        &pushsdb 5							//'m_label_y'
        &pushzerosv
        &pushsdb 6							//'m_focusDirs'
        &pushsdb 7							//'Up/Down'
        &setVariable
        &pushsdb 8							//'m_iconType'
        &pushsdb 9							//'default'
        &setVariable
        &pushsdb 10							//'m_extLabel'
        &pushsdb 11							//''
        &setVariable
        &pushsdb 12							//'m_nTruncateType'
        &pushzerosv
        &pushsdb 13							//'m_label'
        &pushsdb 11							//''
        &setVariable
        &pushsdb 14							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 15							//'m_refFM'
        &pushsdb 16							//'_root.gFM'
        &setVariable
        &pushsdb 17							//'m_initiallySelected'
        &pushfalse
        &setVariable
        &pushsdb 18							//'m_tabIndex'
        &pushbyte 3
        &setVariable
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_contentSymbol'
        &pushsdb 21							//'buttonContentSymbol'
        &setVariable
      &end
    &end // of placeMovieClip 21
  &end // of defineMovieClip 22
&end
